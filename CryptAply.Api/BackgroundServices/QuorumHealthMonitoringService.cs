using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Application.Services;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Api.BackgroundServices
{
    public class QuorumHealthMonitoringService : BackgroundService
    {
        private readonly ILogger<QuorumHealthMonitoringService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(4); // Check every 4 hours

        public QuorumHealthMonitoringService(
            ILogger<QuorumHealthMonitoringService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var cryptoTeamService = scope.ServiceProvider.GetRequiredService<ICryptoTeamService>();
                        var quorumHealthMonitor = scope.ServiceProvider.GetRequiredService<QuorumHealthMonitorService>();
                        var alertingService = scope.ServiceProvider.GetRequiredService<AlertingService>();

                        // Get all active teams
                        var teams = await cryptoTeamService.GetAllActiveTeamsAsync();

                        foreach (var team in teams)
                        {
                            try
                            {
                                // Generate health report
                                var report = await quorumHealthMonitor.GenerateHealthReportAsync(team.Id);

                                // Create alerts based on health score
                                if (report.HealthScore < 50)
                                {
                                    await alertingService.CreateAlertAsync(
                                        $"Critical Quorum Health - {team.Name}",
                                        $"Team quorum health is critical (Score: {report.HealthScore}). " +
                                        $"Critical Issues: {string.Join(", ", report.Warnings.Where(w => w.StartsWith("CRITICAL")))}",
                                        AlertSeverity.High,
                                        "QuorumHealth",
                                        "System");
                                }
                                else if (report.HealthScore < 70)
                                {
                                    await alertingService.CreateAlertAsync(
                                        $"Poor Quorum Health - {team.Name}",
                                        $"Team quorum health needs attention (Score: {report.HealthScore}). " +
                                        $"Issues: {string.Join(", ", report.Warnings)}",
                                        AlertSeverity.Medium,
                                        "QuorumHealth",
                                        "System");
                                }

                                // Store report for historical analysis
                                // TODO: Implement report storage and trending analysis
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error generating health report for team {TeamId}", team.Id);
                            }
                        }
                    }

                    await Task.Delay(_checkInterval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in quorum health monitoring service");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }
    }
}
