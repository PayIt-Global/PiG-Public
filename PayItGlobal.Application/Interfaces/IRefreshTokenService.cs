using System;
using System.Threading.Tasks;

namespace PayItGlobal.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<string> GenerateRefreshToken(int userId, string createdByIp);
        Task<bool> IsValidRefreshToken(string token);
        Task<int> GetUserIdFromRefreshToken(string token);
        Task RevokeRefreshToken(string refreshToken);
    }
}
