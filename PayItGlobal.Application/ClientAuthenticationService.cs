using Microsoft.Maui.Storage;
using PayItGlobal.Application.Interfaces;
using PayItGlobal.Application.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

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
            var request = new AuthenticateRequest { Username = username, Password = password, UserIpAddress = userIpAddress };
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/Token/login", request);

            if (response.IsSuccessStatusCode)
            {
                var tokens = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

                if (tokens != null && tokens.ContainsKey("token") && tokens.ContainsKey("refreshToken"))
                {
                    var jwtToken = tokens["token"];
                    var refreshToken = tokens["refreshToken"];

                    // Decode the JWT token to extract the expiry date
                    var tokenParts = jwtToken.Split('.');
                    if (tokenParts.Length > 1)
                    {
                        var payload = tokenParts[1]; // Base64 URL encoded payload
                        var payloadCorrected = payload.Replace('-', '+').Replace('_', '/');
                        switch (payloadCorrected.Length % 4)
                        {
                            case 2: payloadCorrected += "=="; break;
                            case 3: payloadCorrected += "="; break;
                        }

                        try
                        {
                            var payloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(payloadCorrected));
                            var jwtPayload = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(payloadJson);

                            if (jwtPayload != null)
                            {
                                if (jwtPayload.TryGetValue("exp", out JsonElement expElement) && expElement.TryGetInt64(out long exp))
                                {
                                    var expiryDate = DateTimeOffset.FromUnixTimeSeconds(exp).ToString();
                                    await SecureStorage.SetAsync("jwt_token", jwtToken);
                                    await SecureStorage.SetAsync("refresh_token", refreshToken);
                                    await SecureStorage.SetAsync("jwt_expiry", expiryDate);
                                    return true;
                                }
                                // Handle other claims as needed, converting types appropriately
                            }
                        }
                        catch (FormatException ex)
                        {
                            // Handle or log the FormatException
                        }
                        catch (JsonException ex)
                        {
                            // Handle or log the JsonException
                        }

                        return false;



                    }
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
                    var payload = tokenParts[1]; // Base64 URL encoded payload
                    var payloadCorrected = payload.Replace('-', '+').Replace('_', '/');
                    switch (payloadCorrected.Length % 4)
                    {
                        case 2: payloadCorrected += "=="; break;
                        case 3: payloadCorrected += "="; break;
                    }

                    try
                    {
                        var payloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(payloadCorrected));
                        var jwtPayload = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(payloadJson);

                        if (jwtPayload != null && jwtPayload.TryGetValue("exp", out JsonElement expElement) && expElement.TryGetInt64(out long exp))
                        {
                            var expiryDate = DateTimeOffset.FromUnixTimeSeconds(exp);
                            if (expiryDate > DateTimeOffset.UtcNow)
                            {
                                // Token has not expired, optionally verify its validity with the server
                                return true;
                            }
                        }
                    }
                    catch (FormatException ex)
                    {
                        // Handle or log the FormatException
                    }
                    catch (JsonException ex)
                    {
                        // Handle or log the JsonException
                    }
                }
            }
            return false;
        }

        public async Task<string> RefreshJwtTokenAsync(string refreshToken)
        {
            // Send a request to your API to refresh the JWT using the refresh token
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/Token/refresh", new { refreshToken });

            if (response.IsSuccessStatusCode)
            {
                var tokens = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                if (tokens != null && tokens.ContainsKey("token") && tokens.ContainsKey("refreshToken"))
                {
                    var jwtToken = tokens["token"];
                    var newRefreshToken = tokens["refreshToken"];
                    // Extract the expiry time from the new JWT and store the new tokens and expiry time
                    var expiryTime = ExtractExpiryTimeFromJwt(jwtToken);
                    await StoreTokensAsync(jwtToken, newRefreshToken, expiryTime);

                    return jwtToken;
                }
            }

            // Handle failure (e.g., by logging out the user)
            return null;
        }

        public async Task StoreTokensAsync(string jwtToken, string refreshToken, DateTimeOffset expiryTime)
        {
            await SecureStorage.SetAsync("jwt_token", jwtToken);
            await SecureStorage.SetAsync("refresh_token", refreshToken);
            await SecureStorage.SetAsync("jwt_expiry", expiryTime.ToString());
        }
        public DateTimeOffset ExtractExpiryTimeFromJwt(string jwtToken)
        {
            var payload = jwtToken.Split('.')[1];
            var payloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(payload));
            var jwtPayload = JsonSerializer.Deserialize<Dictionary<string, long>>(payloadJson);

            if (jwtPayload != null && jwtPayload.TryGetValue("exp", out long exp))
            {
                return DateTimeOffset.FromUnixTimeSeconds(exp);
            }

            throw new InvalidOperationException("Invalid JWT token.");
        }

        public async Task<string> GetValidJwtTokenAsync()
        {
            var jwtToken = await SecureStorage.GetAsync("jwt_token");
            var expiryTimeString = await SecureStorage.GetAsync("jwt_expiry");
            var expiryTime = DateTimeOffset.Parse(expiryTimeString);

            // Check if the token is about to expire in the next minute
            if (DateTimeOffset.UtcNow.AddMinutes(1) >= expiryTime)
            {
                var refreshToken = await SecureStorage.GetAsync("refresh_token");
                jwtToken = await RefreshJwtTokenAsync(refreshToken);
                // Assuming RefreshJwtTokenAsync stores the new JWT and expiry time
            }

            return jwtToken;
        }
        public async Task<string> EnsureValidTokenAsync()
        {
            var jwtToken = await SecureStorage.GetAsync("jwt_token");
            var expiryString = await SecureStorage.GetAsync("jwt_expiry");
            var expiryDate = DateTimeOffset.Parse(expiryString);

            // Check if the token is expired or about to expire in the next minute
            if (DateTimeOffset.UtcNow.AddMinutes(1) >= expiryDate)
            {
                // Token is expired or about to expire, refresh it
                var refreshToken = await SecureStorage.GetAsync("refresh_token");
                return await RefreshJwtTokenAsync(refreshToken);
            }
            else
            {
                // Token is still valid
                return jwtToken;
            }
        }
    }

}
