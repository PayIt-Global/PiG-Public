using System;
using System.Collections.Generic;

namespace CryptAplyApp.Infrastructure.Entities
{
    public enum KeyActionType
    {
        Create,
        Activate,
        Rotate,
        Suspend,
        Resume,
        MarkCompromised,
        Archive,
        Delete,
        ModifyAttributes,
        Export,
        GenerateBackup,
        RestoreFromBackup
    }

    public enum KeyActionStatus
    {
        Pending,
        InProgress,
        Approved,
        Rejected,
        Cancelled,
        Completed,
        Failed
    }

    public class KeyAction
    {
        public int Id { get; set; }
        public KeyActionType Type { get; set; }
        public KeyActionStatus Status { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string Reason { get; set; }
        public string Details { get; set; }
        public bool RequiresQuorum { get; set; }
        public int RequiredVotes { get; set; }
        public DateTime? ExpiryDate { get; set; }  // Action expires if not approved
        public string Result { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsEmergency { get; set; }  // For emergency actions that might bypass normal quorum

        // Navigation properties
        public int CryptoKeyId { get; set; }
        public virtual CryptoKey CryptoKey { get; set; }
        
        public int CryptoTeamId { get; set; }
        public virtual CryptoTeam CryptoTeam { get; set; }
        
        public int InitiatorId { get; set; }
        public virtual TeamMember Initiator { get; set; }
        
        public virtual ICollection<KeyActionVote> Votes { get; set; }
        public virtual ICollection<KeyActionAudit> AuditTrail { get; set; }
    }
}
