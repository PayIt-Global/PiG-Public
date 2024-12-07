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
    [Authorize(Roles = "AuditAdmin")]
    public class AuditController : ControllerBase
    {
        private readonly ILogger<AuditController> _logger;
        private readonly AuditLoggingService _auditLoggingService;

        public AuditController(
            ILogger<AuditController> logger,
            AuditLoggingService auditLoggingService)
        {
            _logger = logger;
            _auditLoggingService = auditLoggingService;
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetAuditLogs([FromQuery] AuditLogQuery query)
        {
            try
            {
                var userId = User.Identity.Name;
                await _auditLoggingService.LogActivityAsync(new AuditLogEntry
                {
                    UserId = userId,
                    Action = "ViewAuditLogs",
                    ResourceType = "AuditLog",
                    Timestamp = DateTime.UtcNow,
                    Sensitivity = ActivitySensitivity.Medium
                });

                var logs = await _auditLoggingService.GetAuditLogsAsync(
                    query.StartDate,
                    query.EndDate,
                    query.UserId,
                    query.ResourceType,
                    query.Action,
                    query.Page,
                    query.PageSize);

                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit logs");
                return StatusCode(500, "An error occurred while retrieving audit logs");
            }
        }

        [HttpGet("activity-patterns")]
        public async Task<IActionResult> GetActivityPatterns([FromQuery] string userId)
        {
            try
            {
                var currentUserId = User.Identity.Name;
                await _auditLoggingService.LogActivityAsync(new AuditLogEntry
                {
                    UserId = currentUserId,
                    Action = "ViewActivityPatterns",
                    ResourceId = userId,
                    ResourceType = "UserActivity",
                    Timestamp = DateTime.UtcNow,
                    Sensitivity = ActivitySensitivity.High
                });

                var patterns = await _auditLoggingService.GetUserActivityPatternsAsync(userId);
                return Ok(patterns);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving activity patterns for user {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving activity patterns");
            }
        }

        [HttpGet("suspicious-activities")]
        public async Task<IActionResult> GetSuspiciousActivities([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var userId = User.Identity.Name;
                await _auditLoggingService.LogActivityAsync(new AuditLogEntry
                {
                    UserId = userId,
                    Action = "ViewSuspiciousActivities",
                    ResourceType = "SecurityAlert",
                    Timestamp = DateTime.UtcNow,
                    Sensitivity = ActivitySensitivity.High
                });

                var activities = await _auditLoggingService.GetSuspiciousActivitiesAsync(startDate, endDate);
                return Ok(activities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving suspicious activities");
                return StatusCode(500, "An error occurred while retrieving suspicious activities");
            }
        }
    }

    public class AuditLogQuery
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string UserId { get; set; }
        public string ResourceType { get; set; }
        public string Action { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
