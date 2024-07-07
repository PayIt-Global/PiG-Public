using System.Security.Claims;
using System.Threading.Tasks;

namespace PayItGlobal.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateRefreshToken(Guid userId);
        Task<string?> GenerateAccessTokenFromRefreshToken(string refreshToken);
        Task<ClaimsPrincipal?> RefreshJwtToken(string refreshToken);
        string GenerateJwtToken(Guid userId);
    }
}
