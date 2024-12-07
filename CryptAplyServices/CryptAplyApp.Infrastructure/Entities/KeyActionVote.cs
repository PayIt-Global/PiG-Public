using System;

namespace CryptAplyApp.Infrastructure.Entities
{
    public class KeyActionVote
    {
        public int Id { get; set; }
        public int KeyActionId { get; set; }
        public int TeamMemberId { get; set; }
        public bool Approved { get; set; }
        public string Comment { get; set; }
        public DateTime VoteDate { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public bool WasMFAUsed { get; set; }

        // Navigation properties
        public virtual KeyAction KeyAction { get; set; }
        public virtual TeamMember TeamMember { get; set; }
    }
}
