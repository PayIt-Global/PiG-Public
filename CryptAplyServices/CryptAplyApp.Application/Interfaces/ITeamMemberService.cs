using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Interfaces
{
    public interface ITeamMemberService
    {
        Task<TeamMemberDto> CreateMemberAsync(CreateMemberRequest request);
        Task<TeamMemberDto> UpdateMemberAsync(int memberId, UpdateMemberRequest request);
        Task<TeamMemberDto> GetMemberAsync(int memberId);
        Task<IEnumerable<TeamMemberDto>> GetAllMembersAsync();
        Task<bool> DeactivateMemberAsync(int memberId);
        Task<bool> UpdateMemberRoleAsync(int memberId, string newRole);
        Task<bool> UpdateMFARequirementAsync(int memberId, bool requireMFA);
        Task<bool> UpdateCertificationAsync(int memberId, DateTime newExpiryDate);
        Task<IEnumerable<CryptoTeamDto>> GetMemberTeamsAsync(int memberId);
        Task<IEnumerable<KeyActionDto>> GetMemberPendingVotesAsync(int memberId);
    }
}
