using Microsoft.Extensions.Configuration;
using PayItGlobalApi.Application.Interfaces;
using PayItGlobalApi.Domain.Entities;
using PayItGlobalApi.Domain.Interfaces;
using System.Net.Http.Json;

namespace PayItGlobalApi.Application.Services
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

        public async Task<bool> LoginAsync(string username, string password, string userIpAddress)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_loginUrl, new { username, password });
                if (response.IsSuccessStatusCode)
                {
                    var userId = await response.Content.ReadFromJsonAsync<int>();

                    var jwtToken = _tokenService.GenerateJwtToken(userId);
                    var refreshToken = await _refreshTokenService.GenerateRefreshToken(userId, userIpAddress);

                    var refreshTokenEntity = new RefreshToken
                    {
                        UserId = userId,
                        Token = refreshToken,
                        // Set other necessary properties, such as expiry date
                    };

                    await _refreshTokenRepository.SaveRefreshTokenAsync(refreshTokenEntity);

                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }


        public async Task LogoutAsync()
        {
            // Implementation of logout logic goes here.
            // This could include actions like clearing tokens from storage, 
            // invalidating tokens in the database, etc.
        }


    }
}
