using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Application.Models;
using CryptAplyApp.Application.Interfaces;

namespace CryptAplyApp.Application.Services
{
    public class QuorumHealthMonitorService
    {
        private readonly ILogger<QuorumHealthMonitorService> _logger;
        private readonly ICryptoTeamService _cryptoTeamService;
        private readonly AlertingService _alertingService;
        private readonly AuditLoggingService _auditLoggingService;
        private readonly NotificationServices _notificationService;

        public QuorumHealthMonitorService(
            ILogger<QuorumHealthMonitorService> logger,
            ICryptoTeamService cryptoTeamService,
            AlertingService alertingService,
            AuditLoggingService auditLoggingService,
            NotificationServices notificationService)
        {
            _logger = logger;
            _cryptoTeamService = cryptoTeamService;
            _alertingService = alertingService;
            _auditLoggingService = auditLoggingService;
            _notificationService = notificationService;
        }

        public async Task<QuorumHealthReport> GenerateHealthReportAsync(string teamId)
        {
            var team = await _cryptoTeamService.GetTeamByIdAsync(teamId);
            var activeMembers = await _cryptoTeamService.GetActiveTeamMembersAsync(teamId);
            var backupApprovers = await _cryptoTeamService.GetActiveBackupApproversAsync(teamId);

            var report = new QuorumHealthReport
            {
                TeamId = teamId,
                TeamName = team.Name,
                Timestamp = DateTime.UtcNow,
                RequiredApprovals = team.RequiredApprovals,
                TotalActiveMembers = activeMembers.Count,
                AvailableBackups = backupApprovers.Count,
                RoleDistribution = new Dictionary<string, int>(),
                Warnings = new List<string>(),
                RecommendedActions = new List<string>()
            };

            // Analyze role distribution
            foreach (var member in activeMembers)
            {
                foreach (var role in member.Roles.Where(r => r.IsActive))
                {
                    if (!report.RoleDistribution.ContainsKey(role.RoleType))
                        report.RoleDistribution[role.RoleType] = 0;
                    report.RoleDistribution[role.RoleType]++;
                }
            }

            // Check role-based quorum health
            CheckRoleBasedQuorumHealth(report, team.RequiredRoles);

            // Check certification and training status
            CheckCertificationStatus(report, activeMembers);

            // Check backup coverage
            CheckBackupCoverage(report, activeMembers, backupApprovers);

            // Calculate overall health score
            report.HealthScore = CalculateHealthScore(report);

            // Log report generation
            await _auditLoggingService.LogActivityAsync(new AuditLogEntry
            {
                UserId = "System",
                Action = "QuorumHealthReport",
                ResourceId = teamId,
                ResourceType = "CryptoTeam",
                Timestamp = DateTime.UtcNow,
                Sensitivity = ActivitySensitivity.Medium,
                Details = $"Quorum health report generated. Score: {report.HealthScore}"
            });

            // Create alerts for critical issues
            if (report.Warnings.Any(w => w.Contains("CRITICAL")))
            {
                await _alertingService.CreateAlertAsync(
                    "Critical Quorum Health Issues",
                    $"Team {team.Name} has critical quorum health issues. Score: {report.HealthScore}",
                    AlertSeverity.High,
                    "QuorumHealth",
                    "System");
            }

            return report;
        }

        private void CheckRoleBasedQuorumHealth(QuorumHealthReport report, Dictionary<string, int> requiredRoles)
        {
            foreach (var requirement in requiredRoles)
            {
                var roleType = requirement.Key;
                var requiredCount = requirement.Value;

                if (!report.RoleDistribution.ContainsKey(roleType))
                {
                    report.Warnings.Add($"CRITICAL: No active members with {roleType} role");
                    report.RecommendedActions.Add($"Assign {roleType} role to at least {requiredCount} team members");
                }
                else if (report.RoleDistribution[roleType] < requiredCount)
                {
                    report.Warnings.Add($"WARNING: Insufficient {roleType} role members. Have: {report.RoleDistribution[roleType]}, Need: {requiredCount}");
                    report.RecommendedActions.Add($"Add {requiredCount - report.RoleDistribution[roleType]} more {roleType} members");
                }
            }
        }

        private void CheckCertificationStatus(QuorumHealthReport report, IEnumerable<TeamMember> activeMembers)
        {
            var expiringCerts = activeMembers.Where(m => 
                m.CertificationExpiryDate.HasValue && 
                m.CertificationExpiryDate.Value < DateTime.UtcNow.AddDays(30));

            foreach (var member in expiringCerts)
            {
                var daysUntilExpiry = (member.CertificationExpiryDate.Value - DateTime.UtcNow).Days;
                if (daysUntilExpiry < 0)
                {
                    report.Warnings.Add($"CRITICAL: {member.Name}'s certification has expired");
                }
                else
                {
                    report.Warnings.Add($"WARNING: {member.Name}'s certification expires in {daysUntilExpiry} days");
                }
                report.RecommendedActions.Add($"Renew certification for {member.Name}");
            }
        }

        private void CheckBackupCoverage(
            QuorumHealthReport report, 
            IEnumerable<TeamMember> activeMembers,
            IEnumerable<BackupApprover> backupApprovers)
        {
            var membersWithoutBackup = activeMembers.Where(m => 
                !backupApprovers.Any(ba => ba.TeamMemberId == m.Id));

            foreach (var member in membersWithoutBackup)
            {
                report.Warnings.Add($"WARNING: {member.Name} has no designated backup approver");
                report.RecommendedActions.Add($"Assign backup approver for {member.Name}");
            }
        }

        private int CalculateHealthScore(QuorumHealthReport report)
        {
            var score = 100;

            // Deduct points for warnings based on severity
            score -= report.Warnings.Count(w => w.StartsWith("CRITICAL")) * 20;
            score -= report.Warnings.Count(w => w.StartsWith("WARNING")) * 10;

            // Ensure score stays within 0-100 range
            return Math.Max(0, Math.Min(100, score));
        }
    }

    public class QuorumHealthReport
    {
        public string TeamId { get; set; }
        public string TeamName { get; set; }
        public DateTime Timestamp { get; set; }
        public int RequiredApprovals { get; set; }
        public int TotalActiveMembers { get; set; }
        public int AvailableBackups { get; set; }
        public Dictionary<string, int> RoleDistribution { get; set; }
        public List<string> Warnings { get; set; }
        public List<string> RecommendedActions { get; set; }
        public int HealthScore { get; set; }
    }
}
