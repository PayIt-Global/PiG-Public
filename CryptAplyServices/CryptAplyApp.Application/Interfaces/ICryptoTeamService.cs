using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Interfaces
{
    public interface ICryptoTeamService
    {
        Task<CryptoTeamDto> CreateTeamAsync(CreateTeamRequest request);
        Task<CryptoTeamDto> UpdateTeamAsync(int teamId, UpdateTeamRequest request);
        Task<CryptoTeamDto> GetTeamAsync(int teamId);
        Task<IEnumerable<CryptoTeamDto>> GetAllTeamsAsync();
        Task<bool> DeactivateTeamAsync(int teamId);
        Task<bool> AddMemberToTeamAsync(int teamId, int memberId);
        Task<bool> RemoveMemberFromTeamAsync(int teamId, int memberId);
        Task<IEnumerable<TeamMemberDto>> GetTeamMembersAsync(int teamId);
        Task<bool> UpdateTeamQuorumAsync(int teamId, int newQuorum, bool requireMajority);
        Task<QuorumStatusDto> CheckTeamQuorumAsync(int teamId, int keyActionId);
    }
}
