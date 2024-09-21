using Microsoft.Maui.Storage;

namespace PayItGlobal.Infrastructure.Storage
{
    public class TokenStorage
    {
        public static async Task SaveTokensAsync(string accessToken, string refreshToken)
        {
            await SecureStorage.SetAsync("access_token", accessToken);
            await SecureStorage.SetAsync("refresh_token", refreshToken);
        }

        public static async Task<string?> GetAccessTokenAsync()
        {
            return await SecureStorage.GetAsync("access_token");
        }

        public static async Task<string?> GetRefreshTokenAsync()
        {
            return await SecureStorage.GetAsync("refresh_token");
        }

        public static void RemoveTokens()
        {
            SecureStorage.Remove("access_token");
            SecureStorage.Remove("refresh_token");
        }
    }
}
