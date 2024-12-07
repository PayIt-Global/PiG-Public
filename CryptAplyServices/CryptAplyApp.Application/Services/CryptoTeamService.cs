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
    public class CryptoTeamService : ICryptoTeamService
    {
        private readonly CryptAplyDbContext _context;
        private readonly ILogger<CryptoTeamService> _logger;

        public CryptoTeamService(CryptAplyDbContext context, ILogger<CryptoTeamService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CryptoTeamDto> CreateTeamAsync(CreateTeamRequest request)
        {
            try
            {
                var team = new CryptoTeam
                {
                    Name = request.Name,
                    Description = request.Description,
                    MinimumQuorum = request.MinimumQuorum,
                    RequiresMajority = request.RequiresMajority,
                    CreateDate = DateTime.UtcNow,
                    IsActive = true,
                    CreatedBy = "system" // TODO: Get from current user context
                };

                if (request.InitialMemberIds?.Any() == true)
                {
                    var members = await _context.TeamMembers
                        .Where(m => request.InitialMemberIds.Contains(m.Id))
                        .ToListAsync();
                    team.Members = members;
                }

                _context.CryptoTeams.Add(team);
                await _context.SaveChangesAsync();

                return await GetTeamDtoAsync(team.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating team {TeamName}", request.Name);
                throw;
            }
        }

        public async Task<CryptoTeamDto> UpdateTeamAsync(int teamId, UpdateTeamRequest request)
        {
            var team = await _context.CryptoTeams.FindAsync(teamId);
            if (team == null)
            {
                throw new KeyNotFoundException($"Team with ID {teamId} not found");
            }

            if (request.Description != null)
                team.Description = request.Description;
            
            if (request.MinimumQuorum.HasValue)
                team.MinimumQuorum = request.MinimumQuorum.Value;
            
            if (request.RequiresMajority.HasValue)
                team.RequiresMajority = request.RequiresMajority.Value;

            team.LastModifiedDate = DateTime.UtcNow;
            team.LastModifiedBy = "system"; // TODO: Get from current user context

            await _context.SaveChangesAsync();
            return await GetTeamDtoAsync(teamId);
        }

        public async Task<CryptoTeamDto> GetTeamAsync(int teamId)
        {
            return await GetTeamDtoAsync(teamId);
        }

        public async Task<IEnumerable<CryptoTeamDto>> GetAllTeamsAsync()
        {
            var teams = await _context.CryptoTeams
                .Include(t => t.Members)
                .Include(t => t.ManagedKeys)
                .Include(t => t.KeyActions)
                .ToListAsync();

            return teams.Select(t => new CryptoTeamDto
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

        public async Task<bool> DeactivateTeamAsync(int teamId)
        {
            var team = await _context.CryptoTeams
                .Include(t => t.ManagedKeys)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
                throw new KeyNotFoundException($"Team with ID {teamId} not found");

            if (team.ManagedKeys.Any(k => k.Status == KeyStatus.Active))
                throw new InvalidOperationException("Cannot deactivate team with active keys");

            team.IsActive = false;
            team.LastModifiedDate = DateTime.UtcNow;
            team.LastModifiedBy = "system"; // TODO: Get from current user context

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddMemberToTeamAsync(int teamId, int memberId)
        {
            var team = await _context.CryptoTeams
                .Include(t => t.Members)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
                throw new KeyNotFoundException($"Team with ID {teamId} not found");

            var member = await _context.TeamMembers.FindAsync(memberId);
            if (member == null)
                throw new KeyNotFoundException($"Member with ID {memberId} not found");

            if (team.Members.Any(m => m.Id == memberId))
                return false; // Member already in team

            team.Members.Add(member);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveMemberFromTeamAsync(int teamId, int memberId)
        {
            var team = await _context.CryptoTeams
                .Include(t => t.Members)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
                throw new KeyNotFoundException($"Team with ID {teamId} not found");

            var member = team.Members.FirstOrDefault(m => m.Id == memberId);
            if (member == null)
                return false; // Member not in team

            team.Members.Remove(member);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TeamMemberDto>> GetTeamMembersAsync(int teamId)
        {
            var team = await _context.CryptoTeams
                .Include(t => t.Members)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
                throw new KeyNotFoundException($"Team with ID {teamId} not found");

            return team.Members.Select(m => new TeamMemberDto
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
                CertificationExpiry = m.CertificationExpiry
            });
        }

        public async Task<bool> UpdateTeamQuorumAsync(int teamId, int newQuorum, bool requireMajority)
        {
            var team = await _context.CryptoTeams.FindAsync(teamId);
            if (team == null)
                throw new KeyNotFoundException($"Team with ID {teamId} not found");

            team.MinimumQuorum = newQuorum;
            team.RequiresMajority = requireMajority;
            team.LastModifiedDate = DateTime.UtcNow;
            team.LastModifiedBy = "system"; // TODO: Get from current user context

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<QuorumStatusDto> CheckTeamQuorumAsync(int teamId, int keyActionId)
        {
            var team = await _context.CryptoTeams
                .Include(t => t.Members)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
                throw new KeyNotFoundException($"Team with ID {teamId} not found");

            var action = await _context.KeyActions
                .Include(ka => ka.Votes)
                .ThenInclude(v => v.TeamMember)
                .FirstOrDefaultAsync(ka => ka.Id == keyActionId);

            if (action == null)
                throw new KeyNotFoundException($"Key action with ID {keyActionId} not found");

            var requiredVotes = team.RequiresMajority
                ? Math.Max(team.MinimumQuorum, (int)Math.Ceiling(team.Members.Count * 0.51))
                : team.MinimumQuorum;

            var currentVotes = action.Votes.Count;
            var approvalVotes = action.Votes.Count(v => v.Approved);
            var hasQuorum = approvalVotes >= requiredVotes;

            var votedMemberIds = action.Votes.Select(v => v.TeamMemberId).ToList();
            var pendingVoters = team.Members
                .Where(m => !votedMemberIds.Contains(m.Id))
                .Select(m => m.Name)
                .ToList();

            return new QuorumStatusDto
            {
                RequiredVotes = requiredVotes,
                CurrentVotes = currentVotes,
                HasReachedQuorum = hasQuorum,
                IsMajorityAchieved = approvalVotes > (currentVotes / 2),
                QuorumAchievedAt = hasQuorum ? action.Votes
                    .OrderBy(v => v.VoteDate)
                    .Skip(requiredVotes - 1)
                    .Select(v => v.VoteDate)
                    .FirstOrDefault() : null,
                PendingVoterNames = pendingVoters
            };
        }

        private async Task<CryptoTeamDto> GetTeamDtoAsync(int teamId)
        {
            var team = await _context.CryptoTeams
                .Include(t => t.Members)
                .Include(t => t.ManagedKeys)
                .Include(t => t.KeyActions)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
                throw new KeyNotFoundException($"Team with ID {teamId} not found");

            return new CryptoTeamDto
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                MinimumQuorum = team.MinimumQuorum,
                RequiresMajority = team.RequiresMajority,
                CreateDate = team.CreateDate,
                IsActive = team.IsActive,
                MemberCount = team.Members.Count,
                ActiveKeyCount = team.ManagedKeys.Count(k => k.Status == KeyStatus.Active),
                PendingActionsCount = team.KeyActions.Count(ka => ka.Status == KeyActionStatus.Pending)
            };
        }
    }
}
