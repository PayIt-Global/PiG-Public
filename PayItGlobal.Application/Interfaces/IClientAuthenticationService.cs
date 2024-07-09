namespace PayItGlobal.Application.Interfaces
{
    public interface IClientAuthenticationService
    {
        Task<bool> IsLoggedInAsync();
    }
}
