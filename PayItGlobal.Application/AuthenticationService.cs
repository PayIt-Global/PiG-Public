using System.Net.Http.Json;

namespace PayItGlobal.Application.Services
{
    public class AuthenticationService
    {
        private readonly HttpClient _httpClient;
        private const string LoginUrl = "your_api_base_url/login";
        private const string RefreshTokenUrl = "your_api_base_url/refresh";

        public AuthenticationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync(LoginUrl, new { Username = username, Password = password });
            if (response.IsSuccessStatusCode)
            {
                var tokens = await response.Content.ReadFromJsonAsync<TokenResponse>();
                // Assuming an alternative method to securely store tokens
                if (tokens.Token != null)
                {
                    // Securely store the JWT token
                    StoreTokenSecurely("jwt_token", tokens.Token);
                }
                if (tokens.RefreshToken != null)
                {
                    // Securely store the refresh token
                    StoreTokenSecurely("refresh_token", tokens.RefreshToken);
                }
                return true;
            }
            return false;
        }

        private void StoreTokenSecurely(string key, string value)
        {
            // Implement secure storage logic here
            // This is a placeholder for your secure storage logic
        }

        // Method to refresh JWT token, check authentication status, etc.
    }

    public class TokenResponse
    {
        public string? Token { get; set; } // Made nullable
        public string? RefreshToken { get; set; } // Made nullable
    }
}
