using CryptAplyApp.Application.Services;
using CryptAplyApp.Infrastructure.Repositories;
using CryptAplyApp.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CryptAplyApp.Api.Configuration
{
    public static class AlertingConfiguration
    {
        public static IServiceCollection AddAlertingServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure options
            services.Configure<AlertingOptions>(configuration.GetSection("Alerting"));
            services.Configure<EmailTemplateOptions>(configuration.GetSection("EmailTemplates"));
            services.Configure<SlackOptions>(configuration.GetSection("Slack"));
            services.Configure<PagerDutyOptions>(configuration.GetSection("PagerDuty"));

            // Register HTTP clients
            services.AddHttpClient("slack");
            services.AddHttpClient("pagerduty");

            // Register services
            services.AddScoped<IAlertRepository, AlertRepository>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.AddScoped<ISlackNotificationService, SlackNotificationService>();
            services.AddScoped<IPagerDutyService, PagerDutyService>();
            services.AddScoped<AlertingService>();

            return services;
        }
    }
}
