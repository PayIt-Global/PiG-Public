using PayItGlobal.Application.Interfaces;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Storage;

namespace PayItGlobal.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _loginUrl;
        private const string TokenKey = "AuthToken";
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService; // If you're handling refresh tokens

        public AuthenticationService(HttpClient httpClient, IConfiguration configuration, ITokenService tokenService, IRefreshTokenService refreshTokenService)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
            _refreshTokenService = refreshTokenService;
            var baseUrl = configuration["ApiSettings:BaseUrl"];
            var loginEndpoint = configuration["ApiSettings:LoginEndpoint"];
            _loginUrl = $"{baseUrl}{loginEndpoint}";
        }


        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_loginUrl, new { username, password });
                if (response.IsSuccessStatusCode)
                {
                    // Assuming the user ID is needed for token generation and is returned as part of the login response
                    var userId = await response.Content.ReadFromJsonAsync<int>(); // Simplified for illustration

                    // Generate JWT token
                    var jwtToken = _tokenService.GenerateJwtToken(userId);

                    // Optionally, generate a refresh token if your application uses them
                    var refreshToken = await _refreshTokenService.GenerateRefreshToken(userId, "User IP Address"); // You need to obtain the user's IP address

                    // Store tokens securely
                    await SecureStorage.SetAsync("JwtToken", jwtToken);
                    await SecureStorage.SetAsync("RefreshToken", refreshToken); // If using refresh tokens

                    return true;
                }
                return false;
            }
            catch
            {
                // Handle exceptions (e.g., network errors)
                return false;
            }
        }


        public async Task LogoutAsync()
        {
            // Clear the stored token on logout sadf
            await SecureStorage.SetAsync(TokenKey, null);
        }
    }

    public class TokenResponse
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}
