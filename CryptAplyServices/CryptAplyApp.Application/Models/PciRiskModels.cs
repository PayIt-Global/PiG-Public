using System;
using System.Collections.Generic;

namespace CryptAplyApp.Application.Models
{
    public class RiskAssessment
    {
        public string AssessmentId { get; set; }
        public DateTime AssessmentDate { get; set; }
        public double OverallRiskScore { get; set; }
        public List<RiskFactor> RiskFactors { get; set; }
        public List<string> CompensatingControls { get; set; }
        public string AssessedBy { get; set; }
        public DateTime NextAssessmentDue { get; set; }
        public bool RequiresImmediateAction { get; set; }
    }

    public class RiskFactor
    {
        public string FactorId { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public RiskLevel Level { get; set; }
        public double Weight { get; set; }
        public double Score { get; set; }
        public string Justification { get; set; }
        public List<string> AffectedAssets { get; set; }
        public List<MitigationAction> Mitigations { get; set; }
    }

    public class MitigationAction
    {
        public string ActionId { get; set; }
        public string Description { get; set; }
        public DateTime ImplementationDeadline { get; set; }
        public string ResponsibleParty { get; set; }
        public MitigationStatus Status { get; set; }
        public double EffectivenessScore { get; set; }
        public string ImplementationEvidence { get; set; }
    }

    public class ComplianceEvidence
    {
        public string EvidenceId { get; set; }
        public string RequirementId { get; set; }
        public DateTime CollectionDate { get; set; }
        public string CollectedBy { get; set; }
        public EvidenceType Type { get; set; }
        public string Location { get; set; }
        public DateTime ValidUntil { get; set; }
        public List<string> RelatedFindings { get; set; }
        public bool IsAutomatedCollection { get; set; }
        public string VerificationMethod { get; set; }
    }

    public class ComplianceTrend
    {
        public string TrendId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Metric { get; set; }
        public List<TrendDataPoint> DataPoints { get; set; }
        public TrendDirection Direction { get; set; }
        public double ChangeRate { get; set; }
        public string Analysis { get; set; }
    }

    public class TrendDataPoint
    {
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
        public string Label { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

    public class QsaIntegrationData
    {
        public string IntegrationId { get; set; }
        public string QsaToolName { get; set; }
        public DateTime LastSync { get; set; }
        public Dictionary<string, string> MappedFields { get; set; }
        public List<string> ExportedReports { get; set; }
        public Dictionary<string, string> ApiCredentials { get; set; }
    }

    public enum RiskLevel
    {
        Critical,
        High,
        Medium,
        Low,
        Negligible
    }

    public enum MitigationStatus
    {
        Planned,
        InProgress,
        Implemented,
        Verified,
        Failed,
        Superseded
    }

    public enum EvidenceType
    {
        SystemLog,
        Configuration,
        Screenshot,
        Document,
        Report,
        Attestation,
        AuditTrail
    }

    public enum TrendDirection
    {
        Improving,
        Stable,
        Degrading,
        Fluctuating
    }
}
