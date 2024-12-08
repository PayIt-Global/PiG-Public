using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Application.Models;
using CryptAplyApp.Application.Interfaces;
using CryptAplyApp.Infrastructure.Entities;

namespace CryptAplyApp.Application.Services
{
    public class QuorumManagementService
    {
        private readonly ILogger<QuorumManagementService> _logger;
        private readonly IKeyRepository _keyRepository;
        private readonly ICryptoTeamService _cryptoTeamService;
        private readonly AuditLoggingService _auditLoggingService;
        private readonly AlertingService _alertingService;
        private readonly NotificationServices _notificationService;

        public QuorumManagementService(
            ILogger<QuorumManagementService> logger,
            IKeyRepository keyRepository,
            ICryptoTeamService cryptoTeamService,
            AuditLoggingService auditLoggingService,
            AlertingService alertingService,
            NotificationServices notificationService)
        {
            _logger = logger;
            _keyRepository = keyRepository;
            _cryptoTeamService = cryptoTeamService;
            _auditLoggingService = auditLoggingService;
            _alertingService = alertingService;
            _notificationService = notificationService;
        }

        public async Task<bool> ValidateQuorumAsync(string keyId, IEnumerable<string> approverIds)
        {
            var team = await _cryptoTeamService.GetTeamForKeyAsync(keyId);
            if (team == null)
            {
                throw new InvalidOperationException($"No team found for key {keyId}");
            }

            // Get active backup approvers
            var backupApprovers = await _cryptoTeamService.GetActiveBackupApproversAsync(team.Id);
            
            // Build a map of primary to backup approvers
            var backupMap = backupApprovers.ToDictionary(
                ba => ba.TeamMemberId,
                ba => ba.BackupMemberId);

            // Count valid approvals including backups
            var validApprovals = approverIds.Count(approverId =>
            {
                // Check if approver is a direct team member
                if (team.Members.Any(m => m.Id == approverId))
                    return true;

                // Check if approver is a valid backup for any team member
                return backupMap.Any(kvp => 
                    kvp.Value == approverId && team.Members.Any(m => m.Id == kvp.Key));
            });

            return validApprovals >= team.RequiredApprovals;
        }

        public async Task<bool> CheckQuorumAvailabilityAsync(string teamId)
        {
            var team = await _cryptoTeamService.GetTeamByIdAsync(teamId);
            if (team == null)
                throw new InvalidOperationException($"Team {teamId} not found");

            // Get active members and their backups
            var activeMembers = await _cryptoTeamService.GetActiveTeamMembersAsync(teamId);
            var backupApprovers = await _cryptoTeamService.GetActiveBackupApproversAsync(teamId);

            // Count available approvers (active members + their backups)
            var availableApprovers = activeMembers.Count + 
                backupApprovers.Count(ba => !activeMembers.Any(m => m.Id == ba.TeamMemberId));

            return availableApprovers >= team.RequiredApprovals;
        }

        public async Task<IEnumerable<string>> GetAvailableApproversAsync(string keyId)
        {
            var team = await _cryptoTeamService.GetTeamForKeyAsync(keyId);
            if (team == null)
                throw new InvalidOperationException($"No team found for key {keyId}");

            var activeMembers = await _cryptoTeamService.GetActiveTeamMembersAsync(team.Id);
            var backupApprovers = await _cryptoTeamService.GetActiveBackupApproversAsync(team.Id);

            // Combine active members and their backups
            var availableApprovers = new HashSet<string>();
            
            foreach (var member in activeMembers)
            {
                availableApprovers.Add(member.Id);
            }

            foreach (var backup in backupApprovers)
            {
                // Only add backup if primary member is not active
                if (!activeMembers.Any(m => m.Id == backup.TeamMemberId))
                {
                    availableApprovers.Add(backup.BackupMemberId);
                }
            }

            return availableApprovers;
        }

        public async Task<bool> RegisterBackupApproverAsync(BackupApproverRequest request)
        {
            try
            {
                // Validate both members are in the team
                var team = await _cryptoTeamService.GetTeamByIdAsync(request.TeamId);
                if (!team.Members.Any(m => m.Id == request.TeamMemberId) ||
                    !team.Members.Any(m => m.Id == request.BackupMemberId))
                {
                    throw new InvalidOperationException("Both members must belong to the team");
                }

                // Check for overlapping backup periods
                var existingBackups = await _cryptoTeamService.GetBackupApproversForMemberAsync(
                    request.TeamMemberId,
                    request.StartDate,
                    request.EndDate);

                if (existingBackups.Any())
                {
                    throw new InvalidOperationException("Overlapping backup period exists");
                }

                // Register backup approver
                await _cryptoTeamService.RegisterBackupApproverAsync(
                    request.TeamMemberId,
                    request.BackupMemberId,
                    request.StartDate,
                    request.EndDate,
                    request.Reason);

                // Log the action
                await _auditLoggingService.LogActivityAsync(new AuditLogEntry
                {
                    UserId = request.RequestedBy,
                    Action = "RegisterBackupApprover",
                    ResourceId = request.TeamId,
                    ResourceType = "CryptoTeam",
                    Timestamp = DateTime.UtcNow,
                    Sensitivity = ActivitySensitivity.High,
                    Details = $"Backup approver {request.BackupMemberId} registered for {request.TeamMemberId}"
                });

                // Notify team members
                await _notificationService.NotifyTeamAsync(
                    request.TeamId,
                    "New Backup Approver Registered",
                    $"A new backup approver has been registered for team member {request.TeamMemberId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering backup approver");
                throw;
            }
        }

        public async Task<bool> ValidateEmergencyQuorumAsync(
            string keyId, 
            IEnumerable<string> approverIds, 
            string emergencyReason)
        {
            var team = await _cryptoTeamService.GetTeamForKeyAsync(keyId);
            if (team == null)
                throw new InvalidOperationException($"No team found for key {keyId}");

            // For emergency situations, require security officer approval
            var securityOfficerApproval = approverIds.Any(id => 
                team.Members.Any(m => m.Id == id && m.Role == "SecurityOfficer"));

            if (!securityOfficerApproval)
            {
                _logger.LogWarning("Emergency quorum attempted without security officer approval");
                return false;
            }

            // Require at least half of normal quorum for emergency
            var minimumApprovals = Math.Max(2, team.RequiredApprovals / 2);
            var validApprovals = approverIds.Count(id => team.Members.Any(m => m.Id == id));

            if (validApprovals >= minimumApprovals)
            {
                // Log emergency access
                await _auditLoggingService.LogActivityAsync(new AuditLogEntry
                {
                    UserId = "System",
                    Action = "EmergencyQuorumApproval",
                    ResourceId = keyId,
                    ResourceType = "EncryptionKey",
                    Timestamp = DateTime.UtcNow,
                    Sensitivity = ActivitySensitivity.Critical,
                    Details = $"Emergency quorum approved: {emergencyReason}"
                });

                // Create high-priority alert
                await _alertingService.CreateAlertAsync(
                    "Emergency Quorum Activated",
                    $"Emergency quorum was used for key {keyId}. Reason: {emergencyReason}",
                    AlertSeverity.Critical,
                    "KeyManagement",
                    "System");

                return true;
            }

            return false;
        }
    }

    public class BackupApproverRequest
    {
        public string TeamId { get; set; }
        public string TeamMemberId { get; set; }
        public string BackupMemberId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public string RequestedBy { get; set; }
    }
}
