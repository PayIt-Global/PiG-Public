using PayItGlobalApp.Application.Interfaces;
using PayItGlobalApp.Infrastructure.Storage;
using System.Threading.Tasks;

namespace PayItGlobalApp.Services
{
    public class AuthService
    {
        private readonly IClientTokenService _clientTokenService;

        public AuthService(IClientTokenService clientTokenService)
        {
            _clientTokenService = clientTokenService;
        }

        /// <summary>
        /// Checks if the user is currently logged in.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the user is logged in.</returns>
        public async Task<bool> IsUserLoggedInAsync()
        {
            return await _clientTokenService.IsUserLoggedInAsync();
        }

        /// <summary>
        /// Logs in a user with the specified username and password.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the login was successful.</returns>
        public async Task<bool> LoginAsync(string username, string password)
        {
            return await _clientTokenService.LoginAsync(username, password);
        }

        /// <summary>
        /// Logs out the current user by removing the stored tokens.
        /// </summary>
        public void Logout()
        {
            TokenStorage.RemoveTokens();
        }
    }
}
