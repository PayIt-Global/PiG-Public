using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Application.Services;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AlertController : ControllerBase
    {
        private readonly ILogger<AlertController> _logger;
        private readonly AlertingService _alertingService;
        private readonly AuditLoggingService _auditLoggingService;

        public AlertController(
            ILogger<AlertController> logger,
            AlertingService alertingService,
            AuditLoggingService auditLoggingService)
        {
            _logger = logger;
            _alertingService = alertingService;
            _auditLoggingService = auditLoggingService;
        }

        [HttpPost]
        [Authorize(Roles = "AlertAdmin")]
        public async Task<IActionResult> CreateAlert([FromBody] AlertRequest request)
        {
            try
            {
                var userId = User.Identity.Name;
                await _auditLoggingService.LogActivityAsync(new AuditLogEntry
                {
                    UserId = userId,
                    Action = "CreateAlert",
                    ResourceType = "Alert",
                    Timestamp = DateTime.UtcNow,
                    Sensitivity = ActivitySensitivity.Medium
                });

                var alert = await _alertingService.CreateAlertAsync(
                    request.Title,
                    request.Message,
                    request.Severity,
                    request.Category,
                    userId);

                return Ok(alert);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating alert");
                return StatusCode(500, "An error occurred while creating the alert");
            }
        }

        [HttpGet]
        [Authorize(Roles = "AlertViewer,AlertAdmin")]
        public async Task<IActionResult> GetAlerts([FromQuery] AlertQuery query)
        {
            try
            {
                var userId = User.Identity.Name;
                await _auditLoggingService.LogActivityAsync(new AuditLogEntry
                {
                    UserId = userId,
                    Action = "ViewAlerts",
                    ResourceType = "Alert",
                    Timestamp = DateTime.UtcNow,
                    Sensitivity = ActivitySensitivity.Low
                });

                var alerts = await _alertingService.GetAlertsAsync(
                    query.StartDate,
                    query.EndDate,
                    query.Severity,
                    query.Category,
                    query.Status,
                    query.Page,
                    query.PageSize);

                return Ok(alerts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving alerts");
                return StatusCode(500, "An error occurred while retrieving alerts");
            }
        }

        [HttpPut("{alertId}/acknowledge")]
        [Authorize(Roles = "AlertAdmin")]
        public async Task<IActionResult> AcknowledgeAlert(string alertId)
        {
            try
            {
                var userId = User.Identity.Name;
                await _auditLoggingService.LogActivityAsync(new AuditLogEntry
                {
                    UserId = userId,
                    Action = "AcknowledgeAlert",
                    ResourceId = alertId,
                    ResourceType = "Alert",
                    Timestamp = DateTime.UtcNow,
                    Sensitivity = ActivitySensitivity.Medium
                });

                await _alertingService.AcknowledgeAlertAsync(alertId, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error acknowledging alert {AlertId}", alertId);
                return StatusCode(500, "An error occurred while acknowledging the alert");
            }
        }
    }

    public class AlertRequest
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public AlertSeverity Severity { get; set; }
        public string Category { get; set; }
    }

    public class AlertQuery
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public AlertSeverity? Severity { get; set; }
        public string Category { get; set; }
        public AlertStatus? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
