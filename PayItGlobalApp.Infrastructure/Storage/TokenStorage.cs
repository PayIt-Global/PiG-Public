using Microsoft.Maui.Storage;
using System.Threading.Tasks;

namespace PayItGlobalApp.Infrastructure.Storage
{
    public static class TokenStorage
    {
        /// <summary>
        /// Saves the access token and refresh token securely.
        /// </summary>
        /// <param name="accessToken">The access token to save.</param>
        /// <param name="refreshToken">The refresh token to save.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task SaveTokensAsync(string accessToken, string refreshToken)
        {
            await SecureStorage.SetAsync("access_token", accessToken);
            await SecureStorage.SetAsync("refresh_token", refreshToken);
        }

        /// <summary>
        /// Retrieves the access token from secure storage.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the access token.</returns>
        public static async Task<string?> GetAccessTokenAsync()
        {
            return await SecureStorage.GetAsync("access_token");
        }

        /// <summary>
        /// Retrieves the refresh token from secure storage.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the refresh token.</returns>
        public static async Task<string?> GetRefreshTokenAsync()
        {
            return await SecureStorage.GetAsync("refresh_token");
        }

        /// <summary>
        /// Removes the access token and refresh token from secure storage.
        /// </summary>
        public static void RemoveTokens()
        {
            SecureStorage.Remove("access_token");
            SecureStorage.Remove("refresh_token");
        }
    }
}
