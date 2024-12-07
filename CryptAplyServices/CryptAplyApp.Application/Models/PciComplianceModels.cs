using System;
using System.Collections.Generic;

namespace CryptAplyApp.Application.Models
{
    public class PciComplianceReport
    {
        public string ReportId { get; set; }
        public DateTime ReportingPeriodStart { get; set; }
        public DateTime ReportingPeriodEnd { get; set; }
        public DateTime GenerationDate { get; set; }
        public ComplianceStatus OverallStatus { get; set; }
        public List<ComplianceFinding> Findings { get; set; }
        public KeyManagementMetrics Metrics { get; set; }
        public List<RequirementStatus> RequirementStatuses { get; set; }
        public List<RemediationAction> RemediationActions { get; set; }
        public string GeneratedBy { get; set; }
        public bool RequiresQsaReview { get; set; }
    }

    public class KeyManagementMetrics
    {
        public double KeysWithinPolicyPercentage { get; set; }
        public double SuccessfulOperationsPercentage { get; set; }
        public int FailedOperationsCount { get; set; }
        public int PendingRotationsCount { get; set; }
        public double BackupCompliancePercentage { get; set; }
        public int TotalActiveKeys { get; set; }
        public int KeysRequiringRotation { get; set; }
        public int DualControlEvents { get; set; }
        public int UnauthorizedAccessAttempts { get; set; }
    }

    public class ComplianceFinding
    {
        public string FindingId { get; set; }
        public string Description { get; set; }
        public FindingSeverity Severity { get; set; }
        public string PciRequirement { get; set; }
        public DateTime DetectionDate { get; set; }
        public string AffectedComponents { get; set; }
        public string RemediationPlan { get; set; }
        public DateTime RemediationDeadline { get; set; }
        public string AssignedTo { get; set; }
        public FindingStatus Status { get; set; }
    }

    public class RequirementStatus
    {
        public string RequirementId { get; set; }
        public string Description { get; set; }
        public ComplianceStatus Status { get; set; }
        public DateTime LastVerified { get; set; }
        public string Evidence { get; set; }
        public string Notes { get; set; }
        public List<string> RelatedFindings { get; set; }
    }

    public class RemediationAction
    {
        public string ActionId { get; set; }
        public string Description { get; set; }
        public string RelatedFindingId { get; set; }
        public DateTime Deadline { get; set; }
        public string AssignedTo { get; set; }
        public RemediationStatus Status { get; set; }
        public List<string> Dependencies { get; set; }
        public string CompletionEvidence { get; set; }
    }

    public enum ComplianceStatus
    {
        Compliant,
        PartiallyCompliant,
        NonCompliant,
        UnderReview,
        RemediationInProgress,
        WaiverGranted
    }

    public enum FindingSeverity
    {
        Critical,
        High,
        Medium,
        Low,
        Informational
    }

    public enum FindingStatus
    {
        Open,
        InProgress,
        Remediated,
        Verified,
        WaiverRequested,
        WaiverApproved,
        Closed
    }

    public enum RemediationStatus
    {
        NotStarted,
        InProgress,
        Blocked,
        Completed,
        Verified,
        Failed
    }
}
