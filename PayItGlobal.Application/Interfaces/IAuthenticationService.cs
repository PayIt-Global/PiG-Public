﻿namespace PayItGlobal.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<bool> LoginAsync(string username, string password);
        Task LogoutAsync();
    }
}
