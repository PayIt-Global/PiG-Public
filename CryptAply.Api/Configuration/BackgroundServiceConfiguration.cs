using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CryptAplyApp.Api.BackgroundServices;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Api.Configuration
{
    public static class BackgroundServiceConfiguration
    {
        public static IServiceCollection AddBackgroundServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configure key rotation options
            var keyRotationOptions = new KeyRotationSchedulerOptions();
            configuration.GetSection("KeyRotation").Bind(keyRotationOptions);
            services.AddSingleton(keyRotationOptions);

            // Add background services
            services.AddHostedService<KeyRotationBackgroundService>();
            services.AddHostedService<SecurityMonitoringService>();

            return services;
        }
    }
}
