using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Application.Interfaces;
using CryptAplyApp.Application.Models;
using CryptAplyApp.Infrastructure.Data;
using CryptAplyApp.Infrastructure.Entities;

namespace CryptAplyApp.Application.Services
{
    public class TeamMemberService : ITeamMemberService
    {
        private readonly CryptAplyDbContext _context;
        private readonly ILogger<TeamMemberService> _logger;

        public TeamMemberService(CryptAplyDbContext context, ILogger<TeamMemberService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TeamMemberDto> CreateMemberAsync(CreateMemberRequest request)
        {
            try
            {
                // Check if user already exists
                if (await _context.TeamMembers.AnyAsync(m => m.Email == request.Email || m.UserId == request.UserId))
                {
                    throw new InvalidOperationException("A member with this email or user ID already exists");
                }

                var member = new TeamMember
                {
                    UserId = request.UserId,
                    Name = request.Name,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Role = request.Role,
                    JoinDate = DateTime.UtcNow,
                    RequiresMFA = request.RequiresMFA,
                    IsActive = true,
                    Department = request.Department,
                    BackupContact = request.BackupContact,
                    CertificationExpiry = request.CertificationExpiry
                };

                _context.TeamMembers.Add(member);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created new team member: {Name} ({Email})", member.Name, member.Email);
                return await GetMemberDtoAsync(member.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating team member {Name}", request.Name);
                throw;
            }
        }

        public async Task<TeamMemberDto> UpdateMemberAsync(int memberId, UpdateMemberRequest request)
        {
            var member = await _context.TeamMembers.FindAsync(memberId);
            if (member == null)
            {
                throw new KeyNotFoundException($"Member with ID {memberId} not found");
            }

            try
            {
                if (request.PhoneNumber != null)
                    member.PhoneNumber = request.PhoneNumber;
                
                if (request.Role != null)
                {
                    // Validate role change
                    await ValidateRoleChangeAsync(member, request.Role);
                    member.Role = request.Role;
                }
                
                if (request.RequiresMFA.HasValue)
                    member.RequiresMFA = request.RequiresMFA.Value;
                
                if (request.BackupContact != null)
                    member.BackupContact = request.BackupContact;
                
                if (request.CertificationExpiry.HasValue)
                    member.CertificationExpiry = request.CertificationExpiry;

                member.LastAccessDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated team member: {Name} (ID: {Id})", member.Name, member.Id);
                return await GetMemberDtoAsync(memberId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating team member {Id}", memberId);
                throw;
            }
        }

        public async Task<TeamMemberDto> GetMemberAsync(int memberId)
        {
            return await GetMemberDtoAsync(memberId);
        }

        public async Task<IEnumerable<TeamMemberDto>> GetAllMembersAsync()
        {
            var members = await _context.TeamMembers
                .Include(m => m.Teams)
                .Include(m => m.Votes)
                .ToListAsync();

            return members.Select(m => new TeamMemberDto
            {
                Id = m.Id,
                UserId = m.UserId,
                Name = m.Name,
                Email = m.Email,
                PhoneNumber = m.PhoneNumber,
                Role = m.Role,
                JoinDate = m.JoinDate,
                RequiresMFA = m.RequiresMFA,
                IsActive = m.IsActive,
                CertificationExpiry = m.CertificationExpiry,
                TeamCount = m.Teams.Count,
                PendingVotesCount = m.Votes.Count(v => v.KeyAction.Status == KeyActionStatus.Pending)
            });
        }

        public async Task<bool> DeactivateMemberAsync(int memberId)
        {
            var member = await _context.TeamMembers
                .Include(m => m.Teams)
                .FirstOrDefaultAsync(m => m.Id == memberId);

            if (member == null)
                throw new KeyNotFoundException($"Member with ID {memberId} not found");

            // Check if member is the last admin in any team
            foreach (var team in member.Teams)
            {
                var adminCount = await _context.TeamMembers
                    .CountAsync(m => m.Teams.Contains(team) && m.Role == "Admin" && m.IsActive);
                
                if (adminCount == 1 && member.Role == "Admin")
                    throw new InvalidOperationException($"Cannot deactivate the last admin of team: {team.Name}");
            }

            member.IsActive = false;
            member.LastAccessDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deactivated team member: {Name} (ID: {Id})", member.Name, member.Id);
            return true;
        }

        public async Task<bool> UpdateMemberRoleAsync(int memberId, string newRole)
        {
            var member = await _context.TeamMembers.FindAsync(memberId);
            if (member == null)
                throw new KeyNotFoundException($"Member with ID {memberId} not found");

            await ValidateRoleChangeAsync(member, newRole);
            
            member.Role = newRole;
            member.LastAccessDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated role for member {Name} to {Role}", member.Name, newRole);
            return true;
        }

        public async Task<bool> UpdateMFARequirementAsync(int memberId, bool requireMFA)
        {
            var member = await _context.TeamMembers.FindAsync(memberId);
            if (member == null)
                throw new KeyNotFoundException($"Member with ID {memberId} not found");

            member.RequiresMFA = requireMFA;
            member.LastAccessDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated MFA requirement for member {Name} to {RequireMFA}", member.Name, requireMFA);
            return true;
        }

        public async Task<bool> UpdateCertificationAsync(int memberId, DateTime newExpiryDate)
        {
            var member = await _context.TeamMembers.FindAsync(memberId);
            if (member == null)
                throw new KeyNotFoundException($"Member with ID {memberId} not found");

            if (newExpiryDate <= DateTime.UtcNow)
                throw new ArgumentException("Certification expiry date must be in the future");

            member.CertificationExpiry = newExpiryDate;
            member.LastAccessDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated certification expiry for member {Name} to {ExpiryDate}", 
                member.Name, newExpiryDate);
            return true;
        }

        public async Task<IEnumerable<CryptoTeamDto>> GetMemberTeamsAsync(int memberId)
        {
            var member = await _context.TeamMembers
                .Include(m => m.Teams)
                .ThenInclude(t => t.ManagedKeys)
                .Include(m => m.Teams)
                .ThenInclude(t => t.KeyActions)
                .FirstOrDefaultAsync(m => m.Id == memberId);

            if (member == null)
                throw new KeyNotFoundException($"Member with ID {memberId} not found");

            return member.Teams.Select(t => new CryptoTeamDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                MinimumQuorum = t.MinimumQuorum,
                RequiresMajority = t.RequiresMajority,
                CreateDate = t.CreateDate,
                IsActive = t.IsActive,
                MemberCount = t.Members.Count,
                ActiveKeyCount = t.ManagedKeys.Count(k => k.Status == KeyStatus.Active),
                PendingActionsCount = t.KeyActions.Count(ka => ka.Status == KeyActionStatus.Pending)
            });
        }

        public async Task<IEnumerable<KeyActionDto>> GetMemberPendingVotesAsync(int memberId)
        {
            var member = await _context.TeamMembers
                .Include(m => m.Teams)
                .FirstOrDefaultAsync(m => m.Id == memberId);

            if (member == null)
                throw new KeyNotFoundException($"Member with ID {memberId} not found");

            var teamIds = member.Teams.Select(t => t.Id);
            
            var pendingActions = await _context.KeyActions
                .Include(ka => ka.Votes)
                .Include(ka => ka.Initiator)
                .Where(ka => ka.Status == KeyActionStatus.Pending 
                    && teamIds.Contains(ka.CryptoTeamId)
                    && !ka.Votes.Any(v => v.TeamMemberId == memberId))
                .ToListAsync();

            return pendingActions.Select(a => new KeyActionDto
            {
                Id = a.Id,
                Type = a.Type,
                Status = a.Status,
                RequestDate = a.RequestDate,
                Reason = a.Reason,
                RequiresQuorum = a.RequiresQuorum,
                RequiredVotes = a.RequiredVotes,
                CurrentVotes = a.Votes.Count,
                IsEmergency = a.IsEmergency,
                InitiatorName = a.Initiator.Name,
                Votes = a.Votes.Select(v => new KeyActionVoteDto
                {
                    VoterName = v.TeamMember.Name,
                    Approved = v.Approved,
                    VoteDate = v.VoteDate,
                    UsedMFA = v.WasMFAUsed
                }).ToList()
            });
        }

        private async Task<TeamMemberDto> GetMemberDtoAsync(int memberId)
        {
            var member = await _context.TeamMembers
                .Include(m => m.Teams)
                .Include(m => m.Votes)
                .FirstOrDefaultAsync(m => m.Id == memberId);

            if (member == null)
                throw new KeyNotFoundException($"Member with ID {memberId} not found");

            return new TeamMemberDto
            {
                Id = member.Id,
                UserId = member.UserId,
                Name = member.Name,
                Email = member.Email,
                PhoneNumber = member.PhoneNumber,
                Role = member.Role,
                JoinDate = member.JoinDate,
                RequiresMFA = member.RequiresMFA,
                IsActive = member.IsActive,
                CertificationExpiry = member.CertificationExpiry,
                TeamCount = member.Teams.Count,
                PendingVotesCount = member.Votes.Count(v => v.KeyAction.Status == KeyActionStatus.Pending)
            };
        }

        private async Task ValidateRoleChangeAsync(TeamMember member, string newRole)
        {
            // Check if member is the last admin in any team when downgrading from admin
            if (member.Role == "Admin" && newRole != "Admin")
            {
                var memberTeams = await _context.CryptoTeams
                    .Include(t => t.Members)
                    .Where(t => t.Members.Any(m => m.Id == member.Id))
                    .ToListAsync();

                foreach (var team in memberTeams)
                {
                    var adminCount = team.Members.Count(m => m.Role == "Admin" && m.IsActive);
                    if (adminCount == 1)
                    {
                        throw new InvalidOperationException(
                            $"Cannot change role: Member is the last admin of team {team.Name}");
                    }
                }
            }
        }
    }
}
