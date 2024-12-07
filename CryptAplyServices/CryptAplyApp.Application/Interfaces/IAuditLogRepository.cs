using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptAplyApp.Application.Services;

namespace CryptAplyApp.Application.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<string> CreateLogEntryAsync(AuditLogEntry entry);
        Task<AuditLogEntry> GetLogEntryAsync(string id);
        Task<List<AuditLogEntry>> GetUserActivityAsync(string userId, DateTime startTime, DateTime endTime);
        Task<List<AuditLogEntry>> GetResourceActivityAsync(string resourceId, DateTime startTime, DateTime endTime);
        Task<List<string>> GetUserUsualLocationsAsync(string userId);
        Task<List<AuditLogEntry>> GetSuspiciousActivityAsync(DateTime startTime, DateTime endTime);
        Task<List<AuditLogEntry>> GetFailedActionsAsync(DateTime startTime, DateTime endTime);
        Task<List<AuditLogEntry>> GetHighSensitivityActivityAsync(DateTime startTime, DateTime endTime);
        
        Task CreateLocationHistoryAsync(string userId, string location);
        Task UpdateLocationHistoryAsync(string userId, string location);
        Task<bool> IsLocationApprovedForUserAsync(string userId, string location);
        Task ApproveLocationForUserAsync(string userId, string location, string approvedBy);
        
        Task CreateActivityPatternAsync(string userId, string patternType, string patternValue, double confidence);
        Task UpdateActivityPatternAsync(string userId, string patternType, string patternValue, double confidence);
        Task<List<ActivityPattern>> GetUserActivityPatternsAsync(string userId);
    }
}
