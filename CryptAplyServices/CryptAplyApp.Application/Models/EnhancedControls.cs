using System;
using System.Collections.Generic;

namespace CryptAplyApp.Application.Models
{
    public class EnhancedTimeWindowControls
    {
        public Dictionary<DateTime, string> Holidays { get; set; }
        public bool AllowOperationsDuringHolidays { get; set; }
        public List<MaintenanceWindow> PlannedMaintenance { get; set; }
        public bool BlockDuringMaintenance { get; set; }
        public Dictionary<TimeSpan, int> PeakHourThrottling { get; set; }
        public bool EnableAutomaticLoadBalancing { get; set; }
        public Dictionary<string, TimeWindowControls> RegionSpecificWindows { get; set; }
        public bool EnforceRegionalRestrictions { get; set; }
    }

    public class MaintenanceWindow
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; }
        public MaintenanceType Type { get; set; }
        public string[] AffectedServices { get; set; }
        public bool AllowEmergencyOperations { get; set; }
    }

    public enum MaintenanceType
    {
        Planned,
        Emergency,
        Routine,
        Security
    }

    public class EnhancedQuotaControls
    {
        public bool EnableAdaptiveQuotas { get; set; }
        public double QuotaAdjustmentFactor { get; set; }
        public int MinimumQuota { get; set; }
        public int MaximumQuota { get; set; }
        public Dictionary<string, decimal> OperationCosts { get; set; }
        public decimal DailyBudget { get; set; }
        public bool EnforceBudgetLimits { get; set; }
        public int BurstCapacity { get; set; }
        public TimeSpan BurstDuration { get; set; }
        public bool AllowBursting { get; set; }
        public Dictionary<string, QuotaPriority> OperationPriorities { get; set; }
        public bool EnablePriorityBasedThrottling { get; set; }
    }

    public enum QuotaPriority
    {
        Critical,
        High,
        Medium,
        Low,
        Background
    }

    public class EnhancedNetworkControls
    {
        public List<string> AllowedCountries { get; set; }
        public List<string> BlockedCountries { get; set; }
        public bool EnforceGeographicRestrictions { get; set; }
        public bool EnableAnomalyDetection { get; set; }
        public double AnomalyThreshold { get; set; }
        public TimeSpan PatternLearningPeriod { get; set; }
        public RateLimitingPolicy DdosProtection { get; set; }
        public bool EnableAutomaticBlocking { get; set; }
        public TimeSpan BlockDuration { get; set; }
        public bool RequireContextualIdentity { get; set; }
        public List<string> TrustedIdentityProviders { get; set; }
        public Dictionary<string, List<string>> RequiredClaims { get; set; }
    }

    public class RateLimitingPolicy
    {
        public int MaxRequestsPerSecond { get; set; }
        public int MaxRequestsPerMinute { get; set; }
        public int MaxConcurrentRequests { get; set; }
        public int BlockThreshold { get; set; }
        public TimeSpan BlockDuration { get; set; }
        public bool EnableAdaptiveThrottling { get; set; }
    }

    public class EnhancedAuditControls
    {
        public bool EnableBlockchainAnchoring { get; set; }
        public string BlockchainNetwork { get; set; }
        public TimeSpan AnchorInterval { get; set; }
        public bool EnableAiAnalysis { get; set; }
        public List<string> AnomalyPatterns { get; set; }
        public double AnomalyThreshold { get; set; }
        public List<string> ComplianceFrameworks { get; set; }
        public Dictionary<string, ReportingSchedule> AutomatedReports { get; set; }
        public bool EnableRealTimeAlerts { get; set; }
        public bool EnableForensicLogging { get; set; }
        public int ForensicDataRetentionDays { get; set; }
        public List<string> ForensicEventTypes { get; set; }
    }

    public class ReportingSchedule
    {
        public string ReportName { get; set; }
        public string[] Recipients { get; set; }
        public string Frequency { get; set; }
        public string Format { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }

    public class MachineLearningControls
    {
        public bool EnableAnomalyDetection { get; set; }
        public string ModelEndpoint { get; set; }
        public double ConfidenceThreshold { get; set; }
        public bool EnablePatternLearning { get; set; }
        public int MinimumSamplesRequired { get; set; }
        public TimeSpan LearningPeriod { get; set; }
        public bool EnablePredictiveScaling { get; set; }
        public string PredictionModel { get; set; }
        public int ForecastHorizon { get; set; }
    }

    public class ZeroTrustControls
    {
        public bool RequireContinuousAuthentication { get; set; }
        public TimeSpan SessionValidityPeriod { get; set; }
        public List<string> TrustedIdentityProviders { get; set; }
        public bool RequireDeviceAttestation { get; set; }
        public List<string> AllowedDevicePlatforms { get; set; }
        public bool RequireDeviceCompliance { get; set; }
        public List<ContextualRule> AccessRules { get; set; }
        public bool EnforceContextualAccess { get; set; }
        public Dictionary<string, string[]> RequiredContextAttributes { get; set; }
    }

    public class ContextualRule
    {
        public string RuleName { get; set; }
        public Dictionary<string, string> RequiredAttributes { get; set; }
        public string[] RequiredRoles { get; set; }
        public TimeSpan? ValidityPeriod { get; set; }
        public string[] AllowedOperations { get; set; }
        public RiskLevel MaximumRiskLevel { get; set; }
    }

    public enum RiskLevel
    {
        Low,
        Medium,
        High,
        Critical
    }

    public class RiskScore
    {
        public double Score { get; set; }
        public RiskLevel Level { get; set; }
        public Dictionary<string, double> Factors { get; set; }
        public string[] Mitigations { get; set; }
    }

    public class RiskThresholds
    {
        public double MaxAcceptableScore { get; set; }
        public Dictionary<string, double> OperationSpecificThresholds { get; set; }
        public Dictionary<RiskLevel, string[]> RequiredApprovals { get; set; }
    }

    public class AnomalyScore
    {
        public double Score { get; set; }
        public string[] AnomalousFactors { get; set; }
        public Dictionary<string, double> FactorContributions { get; set; }
        public string[] RecommendedActions { get; set; }
    }

    public class ComplianceReport
    {
        public string Framework { get; set; }
        public DateTime ReportDate { get; set; }
        public Dictionary<string, ComplianceStatus> Requirements { get; set; }
        public List<ComplianceViolation> Violations { get; set; }
        public Dictionary<string, string> Evidence { get; set; }
    }

    public class ComplianceStatus
    {
        public bool IsCompliant { get; set; }
        public string[] Gaps { get; set; }
        public string[] Remediation { get; set; }
        public DateTime LastAssessed { get; set; }
    }

    public class ComplianceViolation
    {
        public string Requirement { get; set; }
        public string Description { get; set; }
        public DateTime DetectedAt { get; set; }
        public string Severity { get; set; }
        public string[] AffectedComponents { get; set; }
    }

    public class AttestationResult
    {
        public bool IsValid { get; set; }
        public string[] Measurements { get; set; }
        public Dictionary<string, string> SecurityState { get; set; }
        public DateTime Timestamp { get; set; }
        public string Signature { get; set; }
    }
}
