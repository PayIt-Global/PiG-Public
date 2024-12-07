using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Application.Services;

namespace CryptAplyApp.Infrastructure.Data
{
    public class DatabaseInitializer
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(
            IServiceProvider serviceProvider,
            ILogger<DatabaseInitializer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task InitializeDatabaseAsync()
        {
            try
            {
                _logger.LogInformation("Starting database initialization");

                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<PciComplianceDbContext>();

                // Apply migrations
                await ApplyMigrationsAsync(dbContext);

                // Seed initial data
                await SeedInitialDataAsync(dbContext);

                _logger.LogInformation("Database initialization completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database");
                throw;
            }
        }

        private async Task ApplyMigrationsAsync(PciComplianceDbContext dbContext)
        {
            _logger.LogInformation("Applying database migrations");
            await dbContext.Database.MigrateAsync();
        }

        private async Task SeedInitialDataAsync(PciComplianceDbContext dbContext)
        {
            await SeedSystemAlertSettingsAsync(dbContext);
        }

        private async Task SeedSystemAlertSettingsAsync(PciComplianceDbContext dbContext)
        {
            // Example of seeding system-wide alert settings
            if (!await dbContext.Alerts.AnyAsync())
            {
                _logger.LogInformation("Seeding system alert settings");

                var systemAlert = new Entities.AlertEntity
                {
                    AlertId = "system-init-" + Guid.NewGuid().ToString(),
                    Title = "System Initialization Complete",
                    Description = "PCI Compliance system has been initialized successfully",
                    Type = AlertType.HighRisk,
                    Severity = AlertSeverity.High,
                    Category = "System",
                    Timestamp = DateTime.UtcNow,
                    Status = AlertStatus.New,
                    Tags = new[] { "system", "initialization" }
                };

                dbContext.Alerts.Add(systemAlert);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
