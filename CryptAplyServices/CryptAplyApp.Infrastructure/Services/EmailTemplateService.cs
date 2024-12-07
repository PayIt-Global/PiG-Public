using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptAplyApp.Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CryptAplyApp.Infrastructure.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly ILogger<EmailTemplateService> _logger;
        private readonly SendGridClient _sendGridClient;
        private readonly EmailTemplateOptions _options;

        public EmailTemplateService(
            ILogger<EmailTemplateService> logger,
            IOptions<EmailTemplateOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            _sendGridClient = new SendGridClient(_options.SendGridApiKey);
        }

        public async Task SendTemplatedEmailAsync(NotificationType templateType, Dictionary<string, string> parameters, List<string> recipients)
        {
            try
            {
                var msg = new SendGridMessage();
                msg.SetFrom(new EmailAddress(_options.FromEmail, _options.FromName));
                
                foreach (var recipient in recipients)
                {
                    msg.AddTo(new EmailAddress(recipient));
                }

                msg.SetTemplateId(GetTemplateId(templateType));
                msg.SetTemplateData(parameters);

                var response = await _sendGridClient.SendEmailAsync(msg);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to send email. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending templated email for template type {TemplateType}", templateType);
                throw;
            }
        }

        private string GetTemplateId(NotificationType templateType)
        {
            return templateType switch
            {
                NotificationType.CriticalAlert => _options.CriticalAlertTemplateId,
                NotificationType.HighPriorityAlert => _options.HighPriorityAlertTemplateId,
                NotificationType.MediumPriorityAlert => _options.MediumPriorityAlertTemplateId,
                NotificationType.LowPriorityAlert => _options.LowPriorityAlertTemplateId,
                NotificationType.DailyDigest => _options.DailyDigestTemplateId,
                NotificationType.WeeklyReport => _options.WeeklyReportTemplateId,
                _ => throw new ArgumentException($"Unknown template type: {templateType}")
            };
        }
    }

    public class EmailTemplateOptions
    {
        public string SendGridApiKey { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public string CriticalAlertTemplateId { get; set; }
        public string HighPriorityAlertTemplateId { get; set; }
        public string MediumPriorityAlertTemplateId { get; set; }
        public string LowPriorityAlertTemplateId { get; set; }
        public string DailyDigestTemplateId { get; set; }
        public string WeeklyReportTemplateId { get; set; }
    }
}
