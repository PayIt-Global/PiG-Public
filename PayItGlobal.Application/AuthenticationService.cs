using PayItGlobal.Application.Interfaces;
using PayItGlobal.Domain.Entities; // Import the namespace for RefreshToken entity
using PayItGlobal.Domain.Interfaces; // Import the namespace for IRefreshTokenRepository
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
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository; // Add this line

        public AuthenticationService(HttpClient httpClient, IConfiguration configuration, ITokenService tokenService, IRefreshTokenService refreshTokenService, IRefreshTokenRepository refreshTokenRepository) // Modify this line
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
            _refreshTokenService = refreshTokenService;
            _refreshTokenRepository = refreshTokenRepository; // Add this line
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

                    // Optionally, handle JWT token storage or return it to the client as needed

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
            // Example logic to check for a stored JWT token and its validity
            var jwtToken = await SecureStorage.GetAsync("jwt_token");
            if (jwtToken != null)
            {
                // Optionally, verify the token's validity with the server
                return true;
            }
            return false;
        }

    }
}
