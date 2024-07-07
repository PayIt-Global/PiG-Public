using System.Security.Claims;
using System.Threading.Tasks;

namespace PayItGlobal.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateRefreshToken(int userId);
        Task<string?> GenerateAccessTokenFromRefreshToken(string refreshToken);
        Task<ClaimsPrincipal?> RefreshJwtToken(string refreshToken);
        string GenerateJwtToken(int userId);
    }
}
