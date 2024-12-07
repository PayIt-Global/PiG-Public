using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CryptAplyApp.Application.Interfaces;
using CryptAplyApp.Application.Services;
using CryptAplyApp.Infrastructure.Data;
using CryptAplyApp.Infrastructure.Entities;

namespace CryptAplyApp.Infrastructure.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly PciComplianceDbContext _dbContext;

        public AuditLogRepository(PciComplianceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> CreateLogEntryAsync(AuditLogEntry entry)
        {
            var entity = new AuditLogEntryEntity
            {
                Id = string.IsNullOrEmpty(entry.Id) ? Guid.NewGuid().ToString() : entry.Id,
                UserId = entry.UserId,
                Action = entry.Action,
                ResourceId = entry.ResourceId,
                ResourceType = entry.ResourceType,
                Timestamp = entry.Timestamp,
                Location = entry.Location,
                IpAddress = entry.IpAddress,
                Sensitivity = entry.Sensitivity,
                Metadata = entry.Metadata
            };

            _dbContext.AuditLogs.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<AuditLogEntry> GetLogEntryAsync(string id)
        {
            var entity = await _dbContext.AuditLogs.FindAsync(id);
            return entity != null ? MapToAuditLogEntry(entity) : null;
        }

        public async Task<List<AuditLogEntry>> GetUserActivityAsync(string userId, DateTime startTime, DateTime endTime)
        {
            var entities = await _dbContext.AuditLogs
                .Where(l => l.UserId == userId && l.Timestamp >= startTime && l.Timestamp <= endTime)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();

            return entities.Select(MapToAuditLogEntry).ToList();
        }

        public async Task<List<AuditLogEntry>> GetResourceActivityAsync(string resourceId, DateTime startTime, DateTime endTime)
        {
            var entities = await _dbContext.AuditLogs
                .Where(l => l.ResourceId == resourceId && l.Timestamp >= startTime && l.Timestamp <= endTime)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();

            return entities.Select(MapToAuditLogEntry).ToList();
        }

        public async Task<List<string>> GetUserUsualLocationsAsync(string userId)
        {
            var locations = await _dbContext.UserLocationHistory
                .Where(h => h.UserId == userId && h.IsApproved)
                .OrderByDescending(h => h.AccessCount)
                .Select(h => h.Location)
                .ToListAsync();

            return locations;
        }

        public async Task<List<AuditLogEntry>> GetSuspiciousActivityAsync(DateTime startTime, DateTime endTime)
        {
            var entities = await _dbContext.AuditLogs
                .Where(l => l.Timestamp >= startTime && l.Timestamp <= endTime)
                .Where(l => l.Sensitivity == ActivitySensitivity.High || !l.WasSuccessful)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();

            return entities.Select(MapToAuditLogEntry).ToList();
        }

        public async Task<List<AuditLogEntry>> GetFailedActionsAsync(DateTime startTime, DateTime endTime)
        {
            var entities = await _dbContext.AuditLogs
                .Where(l => l.Timestamp >= startTime && l.Timestamp <= endTime && !l.WasSuccessful)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();

            return entities.Select(MapToAuditLogEntry).ToList();
        }

        public async Task<List<AuditLogEntry>> GetHighSensitivityActivityAsync(DateTime startTime, DateTime endTime)
        {
            var entities = await _dbContext.AuditLogs
                .Where(l => l.Timestamp >= startTime && l.Timestamp <= endTime && l.Sensitivity == ActivitySensitivity.High)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();

            return entities.Select(MapToAuditLogEntry).ToList();
        }

        public async Task CreateLocationHistoryAsync(string userId, string location)
        {
            var entity = new UserLocationHistoryEntity
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Location = location,
                FirstSeen = DateTime.UtcNow,
                LastSeen = DateTime.UtcNow,
                AccessCount = 1,
                IsApproved = false
            };

            _dbContext.UserLocationHistory.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateLocationHistoryAsync(string userId, string location)
        {
            var entity = await _dbContext.UserLocationHistory
                .FirstOrDefaultAsync(h => h.UserId == userId && h.Location == location);

            if (entity != null)
            {
                entity.LastSeen = DateTime.UtcNow;
                entity.AccessCount++;
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                await CreateLocationHistoryAsync(userId, location);
            }
        }

        public async Task<bool> IsLocationApprovedForUserAsync(string userId, string location)
        {
            var entity = await _dbContext.UserLocationHistory
                .FirstOrDefaultAsync(h => h.UserId == userId && h.Location == location);

            return entity?.IsApproved ?? false;
        }

        public async Task ApproveLocationForUserAsync(string userId, string location, string approvedBy)
        {
            var entity = await _dbContext.UserLocationHistory
                .FirstOrDefaultAsync(h => h.UserId == userId && h.Location == location);

            if (entity != null)
            {
                entity.IsApproved = true;
                entity.ApprovedBy = approvedBy;
                entity.ApprovedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task CreateActivityPatternAsync(string userId, string patternType, string patternValue, double confidence)
        {
            var entity = new ActivityPatternEntity
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                PatternType = patternType,
                PatternValue = patternValue,
                Confidence = confidence,
                LastUpdated = DateTime.UtcNow
            };

            _dbContext.ActivityPatterns.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateActivityPatternAsync(string userId, string patternType, string patternValue, double confidence)
        {
            var entity = await _dbContext.ActivityPatterns
                .FirstOrDefaultAsync(p => p.UserId == userId && p.PatternType == patternType);

            if (entity != null)
            {
                entity.PatternValue = patternValue;
                entity.Confidence = confidence;
                entity.LastUpdated = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                await CreateActivityPatternAsync(userId, patternType, patternValue, confidence);
            }
        }

        public async Task<List<ActivityPattern>> GetUserActivityPatternsAsync(string userId)
        {
            var entities = await _dbContext.ActivityPatterns
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.Confidence)
                .ToListAsync();

            return entities.Select(MapToActivityPattern).ToList();
        }

        private static AuditLogEntry MapToAuditLogEntry(AuditLogEntryEntity entity)
        {
            return new AuditLogEntry
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Action = entity.Action,
                ResourceId = entity.ResourceId,
                ResourceType = entity.ResourceType,
                Timestamp = entity.Timestamp,
                Location = entity.Location,
                IpAddress = entity.IpAddress,
                Sensitivity = entity.Sensitivity,
                Metadata = entity.Metadata
            };
        }

        private static ActivityPattern MapToActivityPattern(ActivityPatternEntity entity)
        {
            return new ActivityPattern
            {
                Id = entity.Id,
                UserId = entity.UserId,
                PatternType = entity.PatternType,
                PatternValue = entity.PatternValue,
                Confidence = entity.Confidence,
                LastUpdated = entity.LastUpdated,
                Metadata = entity.Metadata
            };
        }
    }

    public class ActivityPattern
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string PatternType { get; set; }
        public string PatternValue { get; set; }
        public double Confidence { get; set; }
        public DateTime LastUpdated { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}
