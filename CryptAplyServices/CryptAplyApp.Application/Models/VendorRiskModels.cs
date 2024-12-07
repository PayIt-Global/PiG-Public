using System;
using System.Collections.Generic;

namespace CryptAplyApp.Application.Models
{
    public class VendorProfile
    {
        public string VendorId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public VendorType Type { get; set; }
        public VendorStatus Status { get; set; }
        public DateTime OnboardingDate { get; set; }
        public DateTime LastAssessmentDate { get; set; }
        public List<VendorService> Services { get; set; }
        public List<VendorContact> Contacts { get; set; }
        public List<ComplianceCertification> Certifications { get; set; }
        public VendorRiskScore RiskScore { get; set; }
    }

    public class VendorService
    {
        public string ServiceId { get; set; }
        public string Name { get; set; }
        public ServiceCategory Category { get; set; }
        public string Description { get; set; }
        public DataClassification DataClassification { get; set; }
        public List<string> AffectedSystems { get; set; }
        public List<string> Dependencies { get; set; }
        public bool InvolvesKeyManagement { get; set; }
        public bool RequiresPciCompliance { get; set; }
    }

    public class VendorContact
    {
        public string ContactId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsPrimary { get; set; }
        public ContactType Type { get; set; }
        public List<string> ResponsibleFor { get; set; }
    }

    public class ComplianceCertification
    {
        public string CertificationId { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string IssuingBody { get; set; }
        public string DocumentUrl { get; set; }
        public CertificationStatus Status { get; set; }
    }

    public class VendorRiskScore
    {
        public double OverallScore { get; set; }
        public Dictionary<string, double> CategoryScores { get; set; }
        public List<RiskFinding> Findings { get; set; }
        public DateTime LastUpdated { get; set; }
        public string AssessedBy { get; set; }
        public RiskTrend Trend { get; set; }
    }

    public class RiskFinding
    {
        public string FindingId { get; set; }
        public string Description { get; set; }
        public RiskSeverity Severity { get; set; }
        public string Category { get; set; }
        public DateTime IdentificationDate { get; set; }
        public DateTime? RemediationDate { get; set; }
        public string RemediationPlan { get; set; }
        public string Status { get; set; }
    }

    public enum VendorType
    {
        KeyManagementProvider,
        CertificateAuthority,
        HsmProvider,
        SecurityConsultant,
        ComplianceAuditor,
        SoftwareProvider,
        ServiceProvider
    }

    public enum VendorStatus
    {
        Active,
        UnderReview,
        Suspended,
        Terminated,
        OnboardingInProgress
    }

    public enum ServiceCategory
    {
        KeyManagement,
        Encryption,
        Authentication,
        Compliance,
        Consulting,
        Support,
        Infrastructure
    }

    public enum DataClassification
    {
        PublicData,
        InternalUseOnly,
        Confidential,
        HighlyConfidential,
        Regulated
    }

    public enum ContactType
    {
        Technical,
        Security,
        Compliance,
        Business,
        Emergency,
        Legal
    }

    public enum CertificationStatus
    {
        Active,
        Expired,
        Suspended,
        UnderRenewal,
        Revoked
    }

    public enum RiskSeverity
    {
        Critical,
        High,
        Medium,
        Low,
        Informational
    }

    public enum RiskTrend
    {
        Improving,
        Stable,
        Worsening,
        NeedsReview
    }
}
