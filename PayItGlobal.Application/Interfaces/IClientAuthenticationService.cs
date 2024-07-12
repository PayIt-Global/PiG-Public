namespace PayItGlobal.Application.Interfaces
{
    public interface IClientAuthenticationService
    {
        Task<bool> IsLoggedInAsync();
        Task<bool> LogInAsync(string username, string password, string userIpAddress);
        Task<string> RefreshJwtTokenAsync(string refreshToken);
    }
}
