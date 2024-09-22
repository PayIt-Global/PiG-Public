using PayItGlobalApi.Domain.Interfaces;
using PayItGlobalApi.Infrastructure.Repository;
using PayItGlobalApi.Infrastructure.Interfaces;
using PayItGlobalApi.Application.Interfaces;
using PayItGlobalApi.Infrastructure;
using PayItGlobalApi.Application;
using PayItGlobalApi.Application.Services;
using PayItGlobalApi.Infrastructure.Services;

namespace PayItGlobalApi.Helper
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