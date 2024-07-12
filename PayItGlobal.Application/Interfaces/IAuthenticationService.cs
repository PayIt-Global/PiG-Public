namespace PayItGlobal.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<bool> LoginAsync(string username, string password, string userIpAddress);
        Task LogoutAsync();

    }
}
