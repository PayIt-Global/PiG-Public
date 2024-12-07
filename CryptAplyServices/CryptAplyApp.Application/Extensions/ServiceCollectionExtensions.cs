using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using CryptAplyApp.Application.Models;
using CryptAplyApp.Application.Services;

namespace CryptAplyApp.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKeyRotationScheduler(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<KeyRotationSchedulerOptions>(
                configuration.GetSection("KeyRotationScheduler"));

            services.AddHostedService<KeyRotationScheduler>();

            return services;
        }
    }
}
