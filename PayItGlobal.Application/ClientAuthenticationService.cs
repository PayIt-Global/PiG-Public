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
using PayItGlobal.Application.Models;

namespace PayItGlobal.Application.Services
{
    public class ClientAuthenticationService : IClientAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ClientAuthenticationService(HttpClient httpClient, IApiSettingsService apiSettingsService)
        {
            _httpClient = httpClient;
            _baseUrl = apiSettingsService.GetApiBaseUrl(); // Assuming this method returns the base URL of your API
        }
        public async Task<bool> LoginAsync(string username, string password)
        {
            var request = new AuthenticateRequest { Username = username, Password = password };
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/Token/login", request);

            return response.IsSuccessStatusCode;
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
