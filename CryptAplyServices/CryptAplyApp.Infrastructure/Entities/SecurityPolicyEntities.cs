using System;
using System.Collections.Generic;

namespace CryptAplyApp.Infrastructure.Entities
{
    public class SecurityPolicy
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public PolicyType Type { get; set; }
        public PolicyStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public int Version { get; set; }
        public Dictionary<string, string> Settings { get; set; }
        
        public virtual ICollection<SecurityControl> Controls { get; set; }
        public virtual ICollection<PolicyCompliance> ComplianceRecords { get; set; }
        public virtual ICollection<PolicyException> Exceptions { get; set; }
    }

    public class SecurityControl
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ControlType Type { get; set; }
        public ControlCategory Category { get; set; }
        public string Implementation { get; set; }
        public bool IsAutomated { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
        public bool IsActive { get; set; }
        
        public int SecurityPolicyId { get; set; }
        public virtual SecurityPolicy SecurityPolicy { get; set; }
        public virtual ICollection<ControlAssessment> Assessments { get; set; }
    }

    public class PolicyCompliance
    {
        public int Id { get; set; }
        public DateTime AssessmentDate { get; set; }
        public bool IsCompliant { get; set; }
        public string AssessedBy { get; set; }
        public string Notes { get; set; }
        public Dictionary<string, string> Evidence { get; set; }
        
        public int SecurityPolicyId { get; set; }
        public virtual SecurityPolicy SecurityPolicy { get; set; }
    }

    public class PolicyException
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovalDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string MitigationPlan { get; set; }
        public ExceptionStatus Status { get; set; }
        
        public int SecurityPolicyId { get; set; }
        public virtual SecurityPolicy SecurityPolicy { get; set; }
    }

    public class ControlAssessment
    {
        public int Id { get; set; }
        public DateTime AssessmentDate { get; set; }
        public string AssessedBy { get; set; }
        public ControlEffectiveness Effectiveness { get; set; }
        public string Findings { get; set; }
        public string RecommendedActions { get; set; }
        public Dictionary<string, string> Evidence { get; set; }
        
        public int SecurityControlId { get; set; }
        public virtual SecurityControl SecurityControl { get; set; }
    }

    public enum PolicyType
    {
        AccessControl,
        KeyManagement,
        DataProtection,
        NetworkSecurity,
        ComplianceControl,
        IncidentResponse,
        BusinessContinuity
    }

    public enum PolicyStatus
    {
        Draft,
        UnderReview,
        Active,
        Deprecated,
        Archived
    }

    public enum ControlType
    {
        Preventive,
        Detective,
        Corrective,
        Deterrent
    }

    public enum ControlCategory
    {
        Technical,
        Administrative,
        Physical,
        Operational
    }

    public enum ExceptionStatus
    {
        Pending,
        Approved,
        Rejected,
        Expired
    }

    public enum ControlEffectiveness
    {
        NotEffective,
        PartiallyEffective,
        Effective,
        HighlyEffective
    }
}
