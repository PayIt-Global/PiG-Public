using System.Threading.Tasks;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Services.PolicyEnforcement
{
    public interface IRiskScoreValidator
    {
        Task<RiskScore> CalculateOperationRiskAsync(KeyOperationContext context);
        Task<bool> IsRiskAcceptableAsync(RiskScore score, RiskThresholds thresholds);
        Task LogRiskAssessmentAsync(RiskAssessment assessment);
        Task<RiskMitigationPlan> GenerateMitigationPlanAsync(RiskScore score);
        Task<bool> ValidateRiskMitigationsAsync(string operationId, RiskMitigationPlan plan);
    }

    public interface IBehavioralValidator
    {
        Task<bool> IsOperationPatternNormalAsync(KeyOperationContext context);
        Task<AnomalyScore> DetectAnomaliesAsync(KeyOperationContext context);
        Task UpdateBehavioralProfileAsync(KeyOperationContext context);
        Task<BehavioralProfile> GetProfileAsync(string applicationId);
        Task<AnomalyReport> GenerateAnomalyReportAsync(string timeRange);
    }

    public interface IComplianceValidator
    {
        Task<bool> ValidateComplianceRequirementsAsync(KeyOperationContext context);
        Task<ComplianceReport> GenerateComplianceReportAsync(string timeRange);
        Task<bool> IsOperationCompliantAsync(KeyOperationContext context, string[] frameworks);
        Task<ComplianceEvidence> CollectComplianceEvidenceAsync(string operationId);
        Task<ComplianceStatus> GetFrameworkComplianceStatusAsync(string framework);
    }

    public interface IHardwareSecurityValidator
    {
        Task<bool> ValidateHsmIntegrityAsync();
        Task<bool> VerifyTamperEvidenceAsync();
        Task<bool> ValidateSecureBootAsync();
        Task<AttestationResult> GetHardwareAttestationAsync();
        Task<SecurityState> GetSecurityStateAsync();
        Task<bool> ValidateKeyProtectionAsync(string keyId);
    }

    public interface IZeroTrustValidator
    {
        Task<bool> ValidateContextualAccessAsync(KeyOperationContext context, ZeroTrustControls controls);
        Task<bool> ValidateDeviceComplianceAsync(string deviceId, string[] requiredPolicies);
        Task<bool> ValidateContinuousAuthenticationAsync(string sessionId);
        Task<AccessDecision> EvaluateAccessRequestAsync(AccessRequest request);
    }

    public interface IMachineLearningValidator
    {
        Task<PredictionResult> PredictOperationRiskAsync(KeyOperationContext context);
        Task<bool> IsAnomalousPatternAsync(KeyOperationContext context);
        Task UpdateModelAsync(string modelId, TrainingData data);
        Task<ModelMetrics> GetModelPerformanceAsync(string modelId);
    }

    public class RiskAssessment
    {
        public string OperationId { get; set; }
        public RiskScore Score { get; set; }
        public string[] RiskFactors { get; set; }
        public Dictionary<string, double> FactorWeights { get; set; }
        public RiskMitigationPlan MitigationPlan { get; set; }
    }

    public class RiskMitigationPlan
    {
        public string[] RequiredActions { get; set; }
        public Dictionary<string, string> Compensations { get; set; }
        public string[] RequiredApprovals { get; set; }
        public TimeSpan ValidityPeriod { get; set; }
    }

    public class BehavioralProfile
    {
        public string ApplicationId { get; set; }
        public Dictionary<string, OperationPattern> NormalPatterns { get; set; }
        public AnomalyThresholds Thresholds { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class OperationPattern
    {
        public string Operation { get; set; }
        public TimeSpan[] NormalTimeWindows { get; set; }
        public int AverageFrequency { get; set; }
        public Dictionary<string, double> ContextualFactors { get; set; }
    }

    public class AnomalyReport
    {
        public string TimeRange { get; set; }
        public AnomalyScore[] DetectedAnomalies { get; set; }
        public Dictionary<string, int> AnomalyTypes { get; set; }
        public string[] RecommendedActions { get; set; }
    }

    public class ComplianceEvidence
    {
        public string OperationId { get; set; }
        public Dictionary<string, string> Artifacts { get; set; }
        public string[] AuditTrail { get; set; }
        public Dictionary<string, string> Attestations { get; set; }
    }

    public class SecurityState
    {
        public bool IsSecureBootEnabled { get; set; }
        public bool IsTamperDetected { get; set; }
        public string[] ActiveSecurityModules { get; set; }
        public Dictionary<string, string> SecurityParameters { get; set; }
    }

    public class AccessRequest
    {
        public string PrincipalId { get; set; }
        public string ResourceId { get; set; }
        public string Operation { get; set; }
        public Dictionary<string, string> Context { get; set; }
        public string[] Claims { get; set; }
    }

    public class AccessDecision
    {
        public bool IsAllowed { get; set; }
        public string[] RequiredConditions { get; set; }
        public TimeSpan ValidityPeriod { get; set; }
        public Dictionary<string, string> Restrictions { get; set; }
    }

    public class PredictionResult
    {
        public double RiskScore { get; set; }
        public string[] RiskFactors { get; set; }
        public double Confidence { get; set; }
        public Dictionary<string, double> FeatureImportance { get; set; }
    }

    public class TrainingData
    {
        public string ModelId { get; set; }
        public Dictionary<string, object[]> Features { get; set; }
        public object[] Labels { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

    public class ModelMetrics
    {
        public double Accuracy { get; set; }
        public double Precision { get; set; }
        public double Recall { get; set; }
        public Dictionary<string, double> FeatureImportance { get; set; }
        public DateTime LastTrainingDate { get; set; }
    }
}
