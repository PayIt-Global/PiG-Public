using System;
using System.Collections.Generic;

namespace CryptAplyApp.Application.Models
{
    public class CryptoTeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MinimumQuorum { get; set; }
        public bool RequiresMajority { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsActive { get; set; }
        public int MemberCount { get; set; }
        public int ActiveKeyCount { get; set; }
        public int PendingActionsCount { get; set; }
    }

    public class CreateTeamRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int MinimumQuorum { get; set; }
        public bool RequiresMajority { get; set; }
        public List<int> InitialMemberIds { get; set; }
    }

    public class UpdateTeamRequest
    {
        public string Description { get; set; }
        public int? MinimumQuorum { get; set; }
        public bool? RequiresMajority { get; set; }
    }

    public class QuorumStatusDto
    {
        public int RequiredVotes { get; set; }
        public int CurrentVotes { get; set; }
        public bool HasReachedQuorum { get; set; }
        public bool IsMajorityAchieved { get; set; }
        public DateTime? QuorumAchievedAt { get; set; }
        public List<string> PendingVoterNames { get; set; }
    }
}
