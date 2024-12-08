using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CryptAplyApp.Application.Services;
using CryptAplyApp.Application.Interfaces;
using CryptAplyApp.Infrastructure.Repositories;

namespace CryptAplyApp.Api.Configuration
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register repositories
            services.AddScoped<IKeyRepository, KeyRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();

            // Register services
            services.AddScoped<KeyRotationService>();
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<AuditLoggingService>();
            services.AddScoped<AlertingService>();
            services.AddScoped<UserService>();

            return services;
        }
    }
}
