using PayItGlobalApp.Application.Interfaces;
using PayItGlobalApp.Infrastructure.Storage;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PayItGlobalApp.Application.Services
{
    public class AppTokenService : IAppTokenService
    {
        private readonly HttpClient _httpClient;

        public AppTokenService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> IsUserLoggedInAsync()
        {
            var accessToken = await TokenStorage.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
            {
                return false;
            }

            var isExpired = IsJwtTokenExpired(accessToken);
            if (isExpired)
            {
                var refreshToken = await TokenStorage.GetRefreshTokenAsync();
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return false;
                }

                var newAccessToken = await GenerateAccessTokenFromRefreshToken(refreshToken);
                if (newAccessToken == null)
                {
                    return false;
                }

                await TokenStorage.SaveTokensAsync(newAccessToken, refreshToken);
            }

            return true;
        }

        private bool IsJwtTokenExpired(string token)
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;

            if (jwtToken == null)
                return true;

            return jwtToken.ValidTo < DateTime.UtcNow;
        }

        private async Task<string?> GenerateAccessTokenFromRefreshToken(string refreshToken)
        {
            var response = await _httpClient.PostAsJsonAsync("token/refresh", new { RefreshToken = refreshToken });
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
                return result?.Token;
            }

            return null;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("token/login", new { Username = username, Password = password });
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
                if (result != null)
                {
                    await TokenStorage.SaveTokensAsync(result.Token, result.RefreshToken);
                    return true;
                }
            }

            return false;
        }
    }

    public class TokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
