using System;
using System.Collections.Generic;

namespace CryptAplyApp.Infrastructure.Entities
{
    public class TeamMember
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string LastModifiedBy { get; set; }

        // Role-based properties
        public List<TeamMemberRole> Roles { get; set; } = new List<TeamMemberRole>();
        public string Department { get; set; }
        public string JobTitle { get; set; }
        public string Location { get; set; }
        public DateTime? LastTrainingDate { get; set; }
        public DateTime? CertificationExpiryDate { get; set; }

        // Navigation properties
        public virtual ICollection<BackupApprover> PrimaryApproverFor { get; set; }
        public virtual ICollection<BackupApprover> BackupApproverFor { get; set; }
        public virtual ICollection<KeyActionVote> Votes { get; set; }
    }

    public class TeamMemberRole
    {
        public string Id { get; set; }
        public string TeamMemberId { get; set; }
        public string RoleType { get; set; } // Technical, Business, Security, Compliance, Audit
        public string RoleLevel { get; set; } // Junior, Senior, Lead
        public DateTime AssignedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public string AssignedBy { get; set; }

        // Navigation property
        public virtual TeamMember TeamMember { get; set; }
    }
}
