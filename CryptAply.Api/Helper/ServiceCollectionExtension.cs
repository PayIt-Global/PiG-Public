using CryptAplyApi.Domain.Interfaces;
using CryptAplyApi.Infrastructure.Repository;
using CryptAplyApi.Infrastructure.Interfaces;
using CryptAplyApi.Application.Interfaces;
using CryptAplyApi.Infrastructure;
using CryptAplyApi.Application;
using CryptAplyApi.Application.Services;
using CryptAplyApi.Infrastructure.Services;

namespace CryptAplyApi.Helper
{
    public static class ServiceCollectionExtension
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IMiscRepository, MiscRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        }
    }
}