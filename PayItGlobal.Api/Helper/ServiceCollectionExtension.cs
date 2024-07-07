using PayItGlobal.Domain.Interfaces;
using PayItGlobal.Infrastructure.Repository;
using PayItGlobal.Infrastructure.Interfaces;
using PayItGlobal.Application.Interfaces;
using PayItGlobal.Infrastructure;
using PayItGlobal.Infrastructure.Utilities;
using PayItGlobal.Application;
using PayItGlobal.Application.Services;
using PayItGlobal.Infrastructure.Services;

namespace PayEzPaymentApi.Helper
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