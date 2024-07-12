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
        public async Task<bool> LogInAsync(string username, string password, string userIpAddress)
        {
            // Include the IP address in the request
            var request = new AuthenticateRequest { Username = username, Password = password, UserIpAddress = userIpAddress };
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/Token/login", request);

            if (response.IsSuccessStatusCode)
            {
                // Assuming the response body is a JSON object with "jwtToken" and "refreshToken" properties
                var tokens = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

                if (tokens != null && tokens.ContainsKey("jwtToken") && tokens.ContainsKey("refreshToken"))
                {
                    var jwtToken = tokens["jwtToken"];
                    var refreshToken = tokens["refreshToken"];

                    // Store the tokens securely
                    await SecureStorage.SetAsync("jwt_token", jwtToken);
                    await SecureStorage.SetAsync("refresh_token", refreshToken);

                    return true;
                }
            }

            return false;
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
