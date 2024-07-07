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

        public AuthenticationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
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
                    var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                    if (tokenResponse?.Token != null)
                    {
                        // Store token securely
                        SecureStorage.SetAsync(TokenKey, tokenResponse.Token);
                        return true;
                    }
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
            // Clear the stored token on logout
            await SecureStorage.SetAsync(TokenKey, null);
        }
    }

    public class TokenResponse
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}
