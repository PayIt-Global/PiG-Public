using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CryptAplyApp.Infrastructure.Data;

namespace CryptAplyApp.Api.Configuration
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddPciComplianceDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("PciComplianceDatabase");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string 'PciComplianceDatabase' not found in configuration");
            }

            // Add DbContext
            services.AddDbContext<PciComplianceDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                    
                    sqlOptions.MigrationsAssembly("CryptAplyApp.Infrastructure");
                });
            });

            // Add Database Initializer
            services.AddScoped<DatabaseInitializer>();

            return services;
        }
    }
}
