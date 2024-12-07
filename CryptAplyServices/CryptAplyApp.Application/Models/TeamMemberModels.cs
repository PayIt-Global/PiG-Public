using System;
using System.Collections.Generic;

namespace CryptAplyApp.Application.Models
{
    public class TeamMemberDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public DateTime JoinDate { get; set; }
        public bool RequiresMFA { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CertificationExpiry { get; set; }
        public int TeamCount { get; set; }
        public int PendingVotesCount { get; set; }
    }

    public class CreateMemberRequest
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public bool RequiresMFA { get; set; }
        public string Department { get; set; }
        public string BackupContact { get; set; }
        public DateTime? CertificationExpiry { get; set; }
    }

    public class UpdateMemberRequest
    {
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public bool? RequiresMFA { get; set; }
        public string BackupContact { get; set; }
        public DateTime? CertificationExpiry { get; set; }
    }
}
