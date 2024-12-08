using System;

namespace CryptAplyApp.Infrastructure.Entities
{
    public class BackupApprover
    {
        public string Id { get; set; }
        public string TeamMemberId { get; set; }
        public string BackupMemberId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string LastModifiedBy { get; set; }

        // Navigation properties
        public virtual TeamMember TeamMember { get; set; }
        public virtual TeamMember BackupMember { get; set; }
    }
}
