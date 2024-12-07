using System;
using System.Collections.Generic;

namespace CryptAplyApp.Infrastructure.Entities
{
    public class TeamMember
    {
        public int Id { get; set; }
        public string UserId { get; set; }  // Link to ASP.NET Identity User
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }  // Admin, Manager, Member
        public DateTime JoinDate { get; set; }
        public DateTime? LastAccessDate { get; set; }
        public bool RequiresMFA { get; set; }
        public bool IsActive { get; set; }
        public string Notes { get; set; }
        public string BackupContact { get; set; }
        public string Department { get; set; }
        public DateTime? CertificationExpiry { get; set; }  // For compliance training/certification

        // Navigation properties
        public virtual ICollection<CryptoTeam> Teams { get; set; }
        public virtual ICollection<KeyActionVote> Votes { get; set; }
        public virtual ICollection<KeyAction> InitiatedActions { get; set; }
    }
}
