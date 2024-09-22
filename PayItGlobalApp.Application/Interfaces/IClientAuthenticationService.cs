namespace PayItGlobalApp.Application.Interfaces
{
    public interface IClientAuthenticationService
    {
        /// <summary>
        /// Checks if the user is currently logged in.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the user is logged in.</returns>
        Task<bool> IsLoggedInAsync();

        /// <summary>
        /// Logs in a user with the specified username, password, and user IP address.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <param name="userIpAddress">The IP address of the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the login was successful.</returns>
        Task<bool> LogInAsync(string username, string password, string userIpAddress);

        /// <summary>
        /// Refreshes the JWT token using the provided refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the new JWT token.</returns>
        Task<string> RefreshJwtTokenAsync(string refreshToken);
    }
}

