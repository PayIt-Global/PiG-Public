using System;

namespace CryptAplyApp.Infrastructure.Entities
{
    public class KeyActionAudit
    {
        public int Id { get; set; }
        public int KeyActionId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Event { get; set; }  // Created, StatusChanged, VoteAdded, Completed, etc.
        public string Details { get; set; }
        public string UserId { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }

        // Navigation property
        public virtual KeyAction KeyAction { get; set; }
    }
}
