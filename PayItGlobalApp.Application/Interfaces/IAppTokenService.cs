namespace PayItGlobalApp.Application.Interfaces
{
    public interface IAppTokenService
    {
        /// <summary>
        /// Checks if the user is currently logged in by verifying the access token.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the user is logged in.</returns>
        Task<bool> IsUserLoggedInAsync();

        /// <summary>
        /// Logs in a user with the specified username and password.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the login was successful.</returns>
        Task<bool> LoginAsync(string username, string password);
    }
}
