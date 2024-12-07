using System;
using System.Threading.Tasks;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Application.Interfaces;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly EmailConfiguration _config;
        private readonly EmailTemplateService _templateService;

        public EmailService(
            ILogger<EmailService> logger,
            IOptions<EmailConfiguration> config,
            EmailTemplateService templateService)
        {
            _logger = logger;
            _config = config.Value;
            _templateService = templateService;
        }

        public async Task SendKeyRotationNotificationAsync(KeyRotationNotification notification)
        {
            var template = _templateService.GetTemplate(notification.NotificationType);
            var values = new Dictionary<string, string>
            {
                { "KeyName", notification.KeyName },
                { "KeyId", notification.KeyId.ToString() },
                { "Environment", notification.Environment },
                { "ScheduledRotationDate", notification.ScheduledRotationDate?.ToString("d") ?? "N/A" },
                { "LastRotationDate", notification.LastRotationDate?.ToString("d") ?? "N/A" },
                { "ApplicationList", FormatApplicationList(notification.Applications) }
            };

            var body = _templateService.FormatTemplate(template, values);
            var subject = template.Subject.Replace("{KeyName}", notification.KeyName);

            await SendEmailAsync(notification.TeamMemberEmails, subject, body, template.RequiresMFA);
        }

        public async Task SendKeyRotationReminderAsync(KeyRotationNotification notification)
        {
            var template = _templateService.GetTemplate(notification.NotificationType);
            var values = new Dictionary<string, string>
            {
                { "KeyName", notification.KeyName },
                { "KeyId", notification.KeyId.ToString() },
                { "Environment", notification.Environment },
                { "ScheduledRotationDate", notification.ScheduledRotationDate?.ToString("d") ?? "N/A" },
                { "LastRotationDate", notification.LastRotationDate?.ToString("d") ?? "N/A" },
                { "ApplicationList", FormatApplicationList(notification.Applications) }
            };

            var body = _templateService.FormatTemplate(template, values);
            var subject = template.Subject.Replace("{KeyName}", notification.KeyName);

            await SendEmailAsync(notification.TeamMemberEmails, subject, body, template.RequiresMFA);
        }

        public async Task SendKeyRotationSuccessAsync(KeyRotationNotification notification)
        {
            var template = _templateService.GetTemplate(notification.NotificationType);
            var values = new Dictionary<string, string>
            {
                { "KeyName", notification.KeyName },
                { "KeyId", notification.KeyId.ToString() },
                { "Environment", notification.Environment },
                { "ScheduledRotationDate", notification.ScheduledRotationDate?.ToString("d") ?? "N/A" },
                { "LastRotationDate", notification.LastRotationDate?.ToString("d") ?? "N/A" },
                { "ApplicationList", FormatApplicationList(notification.Applications) }
            };

            var body = _templateService.FormatTemplate(template, values);
            var subject = template.Subject.Replace("{KeyName}", notification.KeyName);

            await SendEmailAsync(notification.TeamMemberEmails, subject, body, template.RequiresMFA);
        }

        public async Task SendKeyRotationFailureAsync(KeyRotationNotification notification)
        {
            var template = _templateService.GetTemplate(notification.NotificationType);
            var values = new Dictionary<string, string>
            {
                { "KeyName", notification.KeyName },
                { "KeyId", notification.KeyId.ToString() },
                { "Environment", notification.Environment },
                { "ScheduledRotationDate", notification.ScheduledRotationDate?.ToString("d") ?? "N/A" },
                { "LastRotationDate", notification.LastRotationDate?.ToString("d") ?? "N/A" },
                { "ApplicationList", FormatApplicationList(notification.Applications) }
            };

            var body = _templateService.FormatTemplate(template, values);
            var subject = template.Subject.Replace("{KeyName}", notification.KeyName);

            await SendEmailAsync(notification.TeamMemberEmails, subject, body, template.RequiresMFA);
        }

        private string FormatApplicationList(Dictionary<string, string> applications)
        {
            if (applications == null || applications.Count == 0)
                return "<li>No affected applications</li>";

            var list = "";
            foreach (var app in applications)
            {
                list += $"<li><strong>{app.Key}:</strong> {app.Value}</li>";
            }
            return list;
        }

        private async Task SendEmailAsync(List<string> toAddresses, string subject, string body, bool requiresMfa = false)
        {
            try
            {
                using var client = new SmtpClient(_config.SmtpServer, _config.SmtpPort)
                {
                    EnableSsl = _config.EnableSsl,
                    Credentials = new System.Net.NetworkCredential(_config.Username, _config.Password)
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(_config.FromEmail, _config.FromName),
                    Subject = subject,
                    Body = WrapEmailInTemplate(body, requiresMfa),
                    IsBodyHtml = true
                };

                foreach (var address in toAddresses)
                {
                    message.To.Add(address);
                }

                if (_config.GlobalCcList != null)
                {
                    foreach (var cc in _config.GlobalCcList)
                    {
                        message.CC.Add(cc);
                    }
                }

                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent successfully. Subject: {Subject}", subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email. Subject: {Subject}", subject);
                throw;
            }
        }

        private string WrapEmailInTemplate(string content, bool requiresMfa)
        {
            var mfaWarning = requiresMfa
                ? "<div class='mfa-warning'>⚠️ This action requires Multi-Factor Authentication</div>"
                : "";

            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <style>
                        body {{ font-family: 'Segoe UI', Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #0078d4; color: white; padding: 20px; margin-bottom: 20px; }}
                        .footer {{ background-color: #f8f9fa; padding: 20px; margin-top: 20px; font-size: 12px; }}
                        .button {{ display: inline-block; padding: 10px 20px; background-color: #0078d4; color: white; 
                                  text-decoration: none; border-radius: 4px; margin: 10px 0; }}
                        .button-critical {{ background-color: #d93025; }}
                        .alert {{ padding: 15px; margin-bottom: 20px; border-radius: 4px; }}
                        .alert-warning {{ background-color: #fff3cd; border: 1px solid #ffeeba; }}
                        .alert-critical {{ background-color: #f8d7da; border: 1px solid #f5c6cb; }}
                        .mfa-warning {{ background-color: #fff3cd; padding: 10px; margin-bottom: 20px; 
                                      border-left: 4px solid #ffc107; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>CryptAply Key Management</h1>
                        </div>
                        {mfaWarning}
                        {content}
                        <div class='footer'>
                            <p>This is an automated message from the CryptAply Key Management System.</p>
                            <p>If you received this email in error, please contact security@company.com</p>
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}
