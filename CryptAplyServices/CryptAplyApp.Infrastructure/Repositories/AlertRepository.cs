using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptAplyApp.Application.Services;
using CryptAplyApp.Infrastructure.Data;
using CryptAplyApp.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptAplyApp.Infrastructure.Repositories
{
    public interface IAlertRepository
    {
        Task<string> CreateAlertAsync(Alert alert);
        Task<Alert> GetAlertAsync(string alertId);
        Task<List<Alert>> GetActiveAlertsAsync();
        Task UpdateAlertAsync(Alert alert);
        Task<List<Alert>> GetAlertsByStatusAsync(AlertStatus status);
        Task<List<Alert>> GetAlertsForDigestAsync(DateTime since);
    }

    public class AlertRepository : IAlertRepository
    {
        private readonly PciComplianceDbContext _dbContext;

        public AlertRepository(PciComplianceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> CreateAlertAsync(Alert alert)
        {
            var entity = new AlertEntity
            {
                AlertId = string.IsNullOrEmpty(alert.AlertId) ? Guid.NewGuid().ToString() : alert.AlertId,
                Title = alert.Title,
                Description = alert.Description,
                Type = alert.Type,
                Severity = alert.Severity,
                Category = alert.Category,
                Timestamp = alert.Timestamp,
                Status = alert.Status,
                Tags = alert.Tags,
                Metadata = alert.Metadata,
                SlackMessageTs = alert.SlackMessageTs
            };

            _dbContext.Alerts.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity.AlertId;
        }

        public async Task<Alert> GetAlertAsync(string alertId)
        {
            var entity = await _dbContext.Alerts.FindAsync(alertId);
            return entity != null ? MapToAlert(entity) : null;
        }

        public async Task<List<Alert>> GetActiveAlertsAsync()
        {
            var entities = await _dbContext.Alerts
                .Where(a => a.Status != AlertStatus.Resolved)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();

            return entities.Select(MapToAlert).ToList();
        }

        public async Task UpdateAlertAsync(Alert alert)
        {
            var entity = await _dbContext.Alerts.FindAsync(alert.AlertId);
            if (entity == null)
                throw new KeyNotFoundException($"Alert {alert.AlertId} not found");

            entity.Title = alert.Title;
            entity.Description = alert.Description;
            entity.Type = alert.Type;
            entity.Severity = alert.Severity;
            entity.Category = alert.Category;
            entity.Status = alert.Status;
            entity.Tags = alert.Tags;
            entity.Metadata = alert.Metadata;
            entity.SlackMessageTs = alert.SlackMessageTs;
            entity.AcknowledgedBy = alert.AcknowledgedBy;
            entity.AcknowledgedAt = alert.AcknowledgedAt;
            entity.ResolvedBy = alert.ResolvedBy;
            entity.ResolvedAt = alert.ResolvedAt;
            entity.Resolution = alert.Resolution;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Alert>> GetAlertsByStatusAsync(AlertStatus status)
        {
            var entities = await _dbContext.Alerts
                .Where(a => a.Status == status)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();

            return entities.Select(MapToAlert).ToList();
        }

        public async Task<List<Alert>> GetAlertsForDigestAsync(DateTime since)
        {
            var entities = await _dbContext.Alerts
                .Where(a => a.Timestamp >= since && a.Severity == AlertSeverity.Low)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();

            return entities.Select(MapToAlert).ToList();
        }

        private static Alert MapToAlert(AlertEntity entity)
        {
            return new Alert
            {
                AlertId = entity.AlertId,
                Title = entity.Title,
                Description = entity.Description,
                Type = entity.Type,
                Severity = entity.Severity,
                Category = entity.Category,
                Timestamp = entity.Timestamp,
                Status = entity.Status,
                Tags = entity.Tags,
                Metadata = entity.Metadata,
                SlackMessageTs = entity.SlackMessageTs,
                AcknowledgedBy = entity.AcknowledgedBy,
                AcknowledgedAt = entity.AcknowledgedAt,
                ResolvedBy = entity.ResolvedBy,
                ResolvedAt = entity.ResolvedAt,
                Resolution = entity.Resolution
            };
        }
    }
}
