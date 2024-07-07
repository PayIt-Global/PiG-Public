using System;
using System.Threading.Tasks;

namespace PayItGlobal.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<string> GenerateRefreshToken(Guid userId, string createdByIp);
        Task<bool> IsValidRefreshToken(string token);
        Task<Guid> GetUserIdFromRefreshToken(string token);
        Task RevokeRefreshToken(string refreshToken);
    }
}
