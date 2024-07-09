using PayItGlobal.Application.Interfaces;
using PayItGlobal.Domain.Entities; 
using PayItGlobal.Domain.Interfaces; 
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Storage;
using System.Text.Json;
using System.Text;

namespace PayItGlobal.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _loginUrl;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository; 

        public AuthenticationService(HttpClient httpClient, IConfiguration configuration, ITokenService tokenService, IRefreshTokenService refreshTokenService, IRefreshTokenRepository refreshTokenRepository) // Modify this line
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
            _refreshTokenService = refreshTokenService;
            _refreshTokenRepository = refreshTokenRepository; 
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
                    var userId = await response.Content.ReadFromJsonAsync<int>(); // Simplified for illustration

                    var jwtToken = _tokenService.GenerateJwtToken(userId);
                    var refreshToken = await _refreshTokenService.GenerateRefreshToken(userId, "User IP Address");

                    // Create a new RefreshToken entity
                    var refreshTokenEntity = new RefreshToken
                    {
                        UserId = userId,
                        Token = refreshToken,
                        // Set other necessary properties, such as expiry date
                    };

                    // Save the refresh token using the repository
                    await _refreshTokenRepository.SaveRefreshTokenAsync(refreshTokenEntity);

                    // Store JWT token in SecureStorage for later validation
                    await SecureStorage.SetAsync("jwt_token", jwtToken);

                    return true;
                }
                return false;
            }
            catch
            {
                // Handle exceptions
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            // Implementation of logout logic goes here.
            // This could include actions like clearing tokens from storage, 
            // invalidating tokens in the database, etc.
        }

        public async Task<bool> IsLoggedInAsync()
        {
            var jwtToken = await SecureStorage.GetAsync("jwt_token");
            if (jwtToken != null)
            {
                // Decode the JWT token to check the expiry date (simplified, actual decoding requires a library)
                var tokenParts = jwtToken.Split('.');
                if (tokenParts.Length > 1)
                {
                    var payload = tokenParts[1]; // Base64 encoded payload
                    var payloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(payload));
                    var jwtPayload = JsonSerializer.Deserialize<Dictionary<string, long>>(payloadJson);

                    if (jwtPayload != null && jwtPayload.TryGetValue("exp", out long exp))
                    {
                        var expiryDate = DateTimeOffset.FromUnixTimeSeconds(exp);
                        if (expiryDate > DateTimeOffset.UtcNow)
                        {
                            // Token has not expired, optionally verify its validity with the server
                            return true;
                        }
                    }
                }
            }
            return false;
        }


    }
}
