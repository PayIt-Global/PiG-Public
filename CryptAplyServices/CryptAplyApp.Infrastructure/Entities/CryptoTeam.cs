using System;
using System.Collections.Generic;

namespace CryptAplyApp.Infrastructure.Entities
{
    public class CryptoTeam
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MinimumQuorum { get; set; }  // Minimum number of members needed for approval
        public bool RequiresMajority { get; set; } // If true, requires >50% of members
        public DateTime CreateDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public string Notes { get; set; }

        // Navigation properties
        public virtual ICollection<TeamMember> Members { get; set; }
        public virtual ICollection<CryptoKey> ManagedKeys { get; set; }
        public virtual ICollection<KeyAction> KeyActions { get; set; }
    }
}
