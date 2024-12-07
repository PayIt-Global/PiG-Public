using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Infrastructure.Entities
{
    public class VendorEntity
    {
        [Key]
        public string VendorId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public VendorType Type { get; set; }
        public VendorStatus Status { get; set; }
        public DateTime OnboardingDate { get; set; }
        public DateTime LastAssessmentDate { get; set; }

        public virtual List<VendorServiceEntity> Services { get; set; }
        public virtual List<VendorContactEntity> Contacts { get; set; }
        public virtual List<VendorCertificationEntity> Certifications { get; set; }
        public virtual VendorRiskScoreEntity RiskScore { get; set; }
    }

    public class VendorServiceEntity
    {
        [Key]
        public string ServiceId { get; set; }
        public string Name { get; set; }
        public ServiceCategory Category { get; set; }
        public string Description { get; set; }
        public DataClassification DataClassification { get; set; }
        public bool InvolvesKeyManagement { get; set; }
        public bool RequiresPciCompliance { get; set; }

        [ForeignKey("VendorId")]
        public string VendorId { get; set; }
        public virtual VendorEntity Vendor { get; set; }

        public string AffectedSystemsJson { get; set; }
        public string DependenciesJson { get; set; }
    }

    public class VendorContactEntity
    {
        [Key]
        public string ContactId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsPrimary { get; set; }
        public ContactType Type { get; set; }

        [ForeignKey("VendorId")]
        public string VendorId { get; set; }
        public virtual VendorEntity Vendor { get; set; }

        public string ResponsibleForJson { get; set; }
    }

    public class VendorCertificationEntity
    {
        [Key]
        public string CertificationId { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string IssuingBody { get; set; }
        public string DocumentUrl { get; set; }
        public CertificationStatus Status { get; set; }

        [ForeignKey("VendorId")]
        public string VendorId { get; set; }
        public virtual VendorEntity Vendor { get; set; }
    }

    public class VendorRiskScoreEntity
    {
        [Key]
        public string RiskScoreId { get; set; }
        public double OverallScore { get; set; }
        public DateTime LastUpdated { get; set; }
        public string AssessedBy { get; set; }
        public RiskTrend Trend { get; set; }

        [ForeignKey("VendorId")]
        public string VendorId { get; set; }
        public virtual VendorEntity Vendor { get; set; }

        public string CategoryScoresJson { get; set; }
        public virtual List<RiskFindingEntity> Findings { get; set; }
    }

    public class RiskFindingEntity
    {
        [Key]
        public string FindingId { get; set; }
        public string Description { get; set; }
        public RiskSeverity Severity { get; set; }
        public string Category { get; set; }
        public DateTime IdentificationDate { get; set; }
        public DateTime? RemediationDate { get; set; }
        public string RemediationPlan { get; set; }
        public string Status { get; set; }

        [ForeignKey("RiskScoreId")]
        public string RiskScoreId { get; set; }
        public virtual VendorRiskScoreEntity RiskScore { get; set; }
    }
}
