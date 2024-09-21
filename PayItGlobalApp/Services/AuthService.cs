using PayItGlobal.Application.Interfaces;
using PayItGlobal.Infrastructure.Storage;
using System.Threading.Tasks;

namespace PayItGlobalApp.Services
{
    public class AuthService
    {
        private readonly ITokenService _tokenService;

        public AuthService(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<bool> IsUserLoggedInAsync()
        {
            var accessToken = await TokenStorage.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
            {
                return false;
            }

            var isExpired = _tokenService.IsJwtTokenExpired(accessToken);
            if (isExpired)
            {
                var refreshToken = await TokenStorage.GetRefreshTokenAsync();
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return false;
                }

                var newAccessToken = await _tokenService.GenerateAccessTokenFromRefreshToken(refreshToken);
                if (newAccessToken == null)
                {
                    return false;
                }

                await TokenStorage.SaveTokensAsync(newAccessToken, refreshToken);
            }

            return true;
        }
    }
}
