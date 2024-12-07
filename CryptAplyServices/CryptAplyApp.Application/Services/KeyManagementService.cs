using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Application.Interfaces;
using CryptAplyApp.Application.Models;
using CryptAplyApp.Infrastructure.Data;
using CryptAplyApp.Infrastructure.Entities;

namespace CryptAplyApp.Application.Services
{
    public class KeyManagementService : IKeyManagementService
    {
        private readonly CryptAplyDbContext _context;
        private readonly ILogger<KeyManagementService> _logger;

        public KeyManagementService(CryptAplyDbContext context, ILogger<KeyManagementService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CryptoKeyDto> CreateKeyAsync(CreateKeyRequest request)
        {
            try
            {
                // Validate managing team exists and is active
                var team = await _context.CryptoTeams
                    .FirstOrDefaultAsync(t => t.Id == request.ManagingTeamId);
                
                if (team == null)
                    throw new KeyNotFoundException($"Team with ID {request.ManagingTeamId} not found");
                
                if (!team.IsActive)
                    throw new InvalidOperationException($"Team {team.Name} is not active");

                // Validate parent key if specified
                if (request.ParentKeyId.HasValue)
                {
                    var parentKey = await _context.CryptoKeys
                        .FirstOrDefaultAsync(k => k.Id == request.ParentKeyId);
                    
                    if (parentKey == null)
                        throw new KeyNotFoundException($"Parent key with ID {request.ParentKeyId} not found");
                    
                    if (parentKey.Status != KeyStatus.Active)
                        throw new InvalidOperationException("Parent key must be active");
                }

                var key = new CryptoKey
                {
                    Name = request.Name,
                    Description = request.Description,
                    Type = request.Type,
                    Status = KeyStatus.Pending,
                    Version = "1.0",
                    KeyVaultUri = GenerateKeyVaultUri(request.Name),
                    Algorithm = request.Algorithm,
                    KeySizeInBits = request.KeySizeInBits,
                    CreateDate = DateTime.UtcNow,
                    RotationPeriodDays = request.RotationPeriodDays,
                    RetentionPeriodDays = request.RetentionPeriodDays,
                    CreatedBy = "system", // TODO: Get from current user context
                    RequiresDoubleAuth = request.RequiresDoubleAuth,
                    IsHSMBacked = request.IsHSMBacked,
                    ParentKeyId = request.ParentKeyId,
                    ManagingTeamId = request.ManagingTeamId
                };

                _context.CryptoKeys.Add(key);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created new key: {Name} (ID: {Id})", key.Name, key.Id);
                return await GetKeyDtoAsync(key.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating key {Name}", request.Name);
                throw;
            }
        }

        public async Task<CryptoKeyDto> GetKeyAsync(int keyId)
        {
            return await GetKeyDtoAsync(keyId);
        }

        public async Task<IEnumerable<CryptoKeyDto>> GetTeamKeysAsync(int teamId)
        {
            var keys = await _context.CryptoKeys
                .Include(k => k.ManagingTeam)
                .Where(k => k.ManagingTeamId == teamId)
                .ToListAsync();

            return keys.Select(k => new CryptoKeyDto
            {
                Id = k.Id,
                Name = k.Name,
                Description = k.Description,
                Type = k.Type,
                Status = k.Status,
                Version = k.Version,
                Algorithm = k.Algorithm,
                KeySizeInBits = k.KeySizeInBits,
                CreateDate = k.CreateDate,
                ActivationDate = k.ActivationDate,
                ExpiryDate = k.ExpiryDate,
                LastRotationDate = k.LastRotationDate,
                NextRotationDate = k.NextRotationDate,
                RequiresDoubleAuth = k.RequiresDoubleAuth,
                IsHSMBacked = k.IsHSMBacked,
                UsageCount = k.UsageCount,
                ManagingTeamName = k.ManagingTeam.Name
            });
        }

        public async Task<KeyActionDto> InitiateKeyActionAsync(CreateKeyActionRequest request)
        {
            var key = await _context.CryptoKeys
                .Include(k => k.ManagingTeam)
                .FirstOrDefaultAsync(k => k.Id == request.KeyId);

            if (key == null)
                throw new KeyNotFoundException($"Key with ID {request.KeyId} not found");

            // Validate action based on current key status
            ValidateKeyActionRequest(key, request);

            var action = new KeyAction
            {
                CryptoKeyId = key.Id,
                CryptoTeamId = key.ManagingTeamId,
                Type = request.Type,
                Status = KeyActionStatus.Pending,
                RequestDate = DateTime.UtcNow,
                Reason = request.Reason,
                Details = request.Details,
                RequiresQuorum = !request.IsEmergency,
                RequiredVotes = request.IsEmergency ? 1 : key.ManagingTeam.MinimumQuorum,
                IsEmergency = request.IsEmergency,
                InitiatorId = GetCurrentUserId() // TODO: Implement actual user context
            };

            if (request.IsEmergency)
            {
                action.ExpiryDate = DateTime.UtcNow.AddHours(4); // Emergency actions expire in 4 hours
            }
            else
            {
                action.ExpiryDate = DateTime.UtcNow.AddDays(7); // Regular actions expire in 7 days
            }

            _context.KeyActions.Add(action);
            await _context.SaveChangesAsync();

            // Create audit entry
            await CreateActionAuditAsync(action.Id, "Created", $"Key action initiated: {request.Type}");

            _logger.LogInformation("Initiated key action: {Type} for key {KeyName} (ID: {KeyId})", 
                request.Type, key.Name, key.Id);

            return await GetKeyActionDtoAsync(action.Id);
        }

        public async Task<KeyActionDto> GetKeyActionAsync(int actionId)
        {
            return await GetKeyActionDtoAsync(actionId);
        }

        public async Task<bool> VoteOnKeyActionAsync(int actionId, KeyActionVoteRequest request)
        {
            var action = await _context.KeyActions
                .Include(ka => ka.Votes)
                .Include(ka => ka.CryptoKey)
                .Include(ka => ka.CryptoTeam)
                .FirstOrDefaultAsync(ka => ka.Id == actionId);

            if (action == null)
                throw new KeyNotFoundException($"Key action with ID {actionId} not found");

            if (action.Status != KeyActionStatus.Pending)
                throw new InvalidOperationException("Can only vote on pending actions");

            if (action.ExpiryDate <= DateTime.UtcNow)
                throw new InvalidOperationException("Action has expired");

            var currentUserId = GetCurrentUserId(); // TODO: Implement actual user context

            if (action.Votes.Any(v => v.TeamMemberId == currentUserId))
                throw new InvalidOperationException("User has already voted on this action");

            var vote = new KeyActionVote
            {
                KeyActionId = actionId,
                TeamMemberId = currentUserId,
                Approved = request.Approved,
                Comment = request.Comment,
                VoteDate = DateTime.UtcNow,
                IpAddress = GetCurrentIpAddress(), // TODO: Implement
                UserAgent = GetCurrentUserAgent(), // TODO: Implement
                WasMFAUsed = request.UseMFA
            };

            _context.KeyActionVotes.Add(vote);

            // Check if action can be completed
            await UpdateActionStatusAsync(action);

            await _context.SaveChangesAsync();

            // Create audit entry
            await CreateActionAuditAsync(actionId, "Vote Added", 
                $"Vote {(request.Approved ? "approved" : "rejected")} by user {currentUserId}");

            _logger.LogInformation("Vote recorded for action {ActionId}: {Approved}", actionId, request.Approved);
            return true;
        }

        public async Task<KeyActionStatusDto> GetKeyActionStatusAsync(int actionId)
        {
            var action = await _context.KeyActions
                .Include(ka => ka.Votes)
                .FirstOrDefaultAsync(ka => ka.Id == actionId);

            if (action == null)
                throw new KeyNotFoundException($"Key action with ID {actionId} not found");

            return new KeyActionStatusDto
            {
                Status = action.Status,
                ApprovalCount = action.Votes.Count(v => v.Approved),
                RejectionCount = action.Votes.Count(v => !v.Approved),
                HasReachedQuorum = action.Votes.Count(v => v.Approved) >= action.RequiredVotes,
                CompletionDate = action.CompletionDate,
                Result = action.Result
            };
        }

        public async Task<IEnumerable<KeyActionDto>> GetPendingActionsAsync(int teamId)
        {
            var actions = await _context.KeyActions
                .Include(ka => ka.CryptoKey)
                .Include(ka => ka.Votes)
                .Include(ka => ka.Initiator)
                .Where(ka => ka.CryptoTeamId == teamId && ka.Status == KeyActionStatus.Pending)
                .OrderByDescending(ka => ka.IsEmergency)
                .ThenBy(ka => ka.RequestDate)
                .ToListAsync();

            return actions.Select(a => new KeyActionDto
            {
                Id = a.Id,
                Type = a.Type,
                Status = a.Status,
                RequestDate = a.RequestDate,
                Reason = a.Reason,
                RequiresQuorum = a.RequiresQuorum,
                RequiredVotes = a.RequiredVotes,
                CurrentVotes = a.Votes.Count,
                IsEmergency = a.IsEmergency,
                InitiatorName = a.Initiator.Name,
                Votes = a.Votes.Select(v => new KeyActionVoteDto
                {
                    VoterName = v.TeamMember.Name,
                    Approved = v.Approved,
                    VoteDate = v.VoteDate,
                    UsedMFA = v.WasMFAUsed
                }).ToList()
            });
        }

        public async Task<bool> RotateKeyAsync(int keyId, RotateKeyRequest request)
        {
            var key = await _context.CryptoKeys
                .Include(k => k.ManagingTeam)
                .FirstOrDefaultAsync(k => k.Id == keyId);

            if (key == null)
                throw new KeyNotFoundException($"Key with ID {keyId} not found");

            if (key.Status != KeyStatus.Active)
                throw new InvalidOperationException("Can only rotate active keys");

            // Create rotation action
            var action = new KeyAction
            {
                CryptoKeyId = keyId,
                CryptoTeamId = key.ManagingTeamId,
                Type = KeyActionType.Rotate,
                Status = KeyActionStatus.Pending,
                RequestDate = DateTime.UtcNow,
                Reason = request.Reason,
                RequiresQuorum = !request.IsEmergency,
                RequiredVotes = request.IsEmergency ? 1 : key.ManagingTeam.MinimumQuorum,
                IsEmergency = request.IsEmergency,
                InitiatorId = GetCurrentUserId(),
                ExpiryDate = request.IsEmergency ? DateTime.UtcNow.AddHours(4) : DateTime.UtcNow.AddDays(7)
            };

            if (request.ScheduledDate.HasValue)
            {
                if (request.ScheduledDate.Value <= DateTime.UtcNow)
                    throw new ArgumentException("Scheduled rotation date must be in the future");
                
                action.Details = $"Scheduled for: {request.ScheduledDate.Value:yyyy-MM-dd HH:mm:ss UTC}";
            }

            _context.KeyActions.Add(action);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Key rotation initiated for key {KeyName} (ID: {KeyId})", key.Name, keyId);
            return true;
        }

        private async Task UpdateActionStatusAsync(KeyAction action)
        {
            var approvalCount = action.Votes.Count(v => v.Approved);
            var rejectionCount = action.Votes.Count(v => !v.Approved);
            var totalVotes = approvalCount + rejectionCount;

            // Check if action has been rejected
            if (rejectionCount > (action.RequiredVotes / 2))
            {
                action.Status = KeyActionStatus.Rejected;
                action.CompletionDate = DateTime.UtcNow;
                action.Result = "Action rejected by majority vote";
                return;
            }

            // Check if action has been approved
            if (approvalCount >= action.RequiredVotes)
            {
                action.Status = KeyActionStatus.Approved;
                action.CompletionDate = DateTime.UtcNow;
                action.Result = "Action approved with required votes";

                // Process the approved action
                await ProcessApprovedActionAsync(action);
            }
        }

        private async Task ProcessApprovedActionAsync(KeyAction action)
        {
            try
            {
                switch (action.Type)
                {
                    case KeyActionType.Create:
                        await ActivateKeyAsync(action.CryptoKeyId);
                        break;
                    case KeyActionType.Rotate:
                        await ProcessKeyRotationAsync(action);
                        break;
                    case KeyActionType.Suspend:
                        await SuspendKeyAsync(action.CryptoKeyId);
                        break;
                    case KeyActionType.Resume:
                        await ResumeKeyAsync(action.CryptoKeyId);
                        break;
                    case KeyActionType.Archive:
                        await ArchiveKeyAsync(action.CryptoKeyId);
                        break;
                    // Add other action types as needed
                }

                action.Status = KeyActionStatus.Completed;
                action.CompletionDate = DateTime.UtcNow;
                await CreateActionAuditAsync(action.Id, "Completed", "Action processed successfully");
            }
            catch (Exception ex)
            {
                action.Status = KeyActionStatus.Failed;
                action.ErrorMessage = ex.Message;
                await CreateActionAuditAsync(action.Id, "Failed", $"Action processing failed: {ex.Message}");
                throw;
            }
        }

        private async Task ProcessKeyRotationAsync(KeyAction action)
        {
            var key = await _context.CryptoKeys.FindAsync(action.CryptoKeyId);
            if (key == null)
                throw new KeyNotFoundException($"Key with ID {action.CryptoKeyId} not found");

            try
            {
                // Start rotation
                key.Status = KeyStatus.PendingRotation;
                await _context.SaveChangesAsync();
                await CreateActionAuditAsync(action.Id, "RotationStarted", "Key rotation process started");

                // Generate new version number
                var newVersion = IncrementVersion(key.Version);
                var newKeyVaultUri = await GenerateNewKeyVersionAsync(key.KeyVaultUri, newVersion);

                // Create backup of current key
                await BackupCurrentKeyAsync(key);

                // Update key metadata
                key.Version = newVersion;
                key.KeyVaultUri = newKeyVaultUri;
                key.LastRotationDate = DateTime.UtcNow;
                key.NextRotationDate = DateTime.UtcNow.AddDays(key.RotationPeriodDays);
                key.Status = KeyStatus.Active;
                key.LastModifiedBy = GetCurrentUserId().ToString();

                await _context.SaveChangesAsync();
                await CreateActionAuditAsync(action.Id, "RotationCompleted", 
                    $"Key rotated to version {newVersion}");

                _logger.LogInformation("Key rotation completed for key {KeyId} to version {Version}", 
                    key.Id, newVersion);
            }
            catch (Exception ex)
            {
                key.Status = KeyStatus.Active; // Revert status on failure
                await _context.SaveChangesAsync();
                await CreateActionAuditAsync(action.Id, "RotationFailed", 
                    $"Key rotation failed: {ex.Message}");

                _logger.LogError(ex, "Key rotation failed for key {KeyId}", key.Id);
                throw;
            }
        }

        private async Task BackupCurrentKeyAsync(CryptoKey key)
        {
            try
            {
                // Create backup record
                var backup = new KeyBackup
                {
                    CryptoKeyId = key.Id,
                    Version = key.Version,
                    KeyVaultUri = key.KeyVaultUri,
                    BackupDate = DateTime.UtcNow,
                    BackupBy = GetCurrentUserId().ToString(),
                    RetentionDate = DateTime.UtcNow.AddDays(key.RetentionPeriodDays)
                };

                _context.KeyBackups.Add(backup);
                await _context.SaveChangesAsync();

                await CreateActionAuditAsync(0, "BackupCreated", 
                    $"Backup created for key {key.Name} version {key.Version}");

                _logger.LogInformation("Backup created for key {KeyName} version {Version}", 
                    key.Name, key.Version);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create backup for key {KeyId} version {Version}", 
                    key.Id, key.Version);
                throw;
            }
        }

        private string IncrementVersion(string currentVersion)
        {
            if (Version.TryParse(currentVersion, out Version version))
            {
                return new Version(version.Major, version.Minor, version.Build + 1).ToString();
            }
            
            // If current version is not in the expected format, start with 1.0.0
            return "1.0.0";
        }

        private async Task<string> GenerateNewKeyVersionAsync(string currentKeyVaultUri, string newVersion)
        {
            // TODO: Implement actual Key Vault key version generation
            // This is a placeholder that maintains the URI structure
            var baseUri = currentKeyVaultUri.Split('/').Take(4).Join("/");
            return $"{baseUri}/{newVersion}";
        }

        private class KeyBackup
        {
            public int Id { get; set; }
            public int CryptoKeyId { get; set; }
            public string Version { get; set; }
            public string KeyVaultUri { get; set; }
            public DateTime BackupDate { get; set; }
            public string BackupBy { get; set; }
            public DateTime RetentionDate { get; set; }
        }

        private async Task CreateActionAuditAsync(int actionId, string eventType, string details)
        {
            var audit = new KeyActionAudit
            {
                KeyActionId = actionId,
                Timestamp = DateTime.UtcNow,
                Event = eventType,
                Details = details,
                UserId = GetCurrentUserId(), // TODO: Implement
                IpAddress = GetCurrentIpAddress(), // TODO: Implement
                UserAgent = GetCurrentUserAgent() // TODO: Implement
            };

            _context.KeyActionAudits.Add(audit);
            await _context.SaveChangesAsync();
        }

        private void ValidateKeyActionRequest(CryptoKey key, CreateKeyActionRequest request)
        {
            switch (request.Type)
            {
                case KeyActionType.Create when key.Status != KeyStatus.Pending:
                    throw new InvalidOperationException("Can only create keys in pending status");
                case KeyActionType.Rotate when key.Status != KeyStatus.Active:
                    throw new InvalidOperationException("Can only rotate active keys");
                case KeyActionType.Suspend when key.Status != KeyStatus.Active:
                    throw new InvalidOperationException("Can only suspend active keys");
                case KeyActionType.Resume when key.Status != KeyStatus.Suspended:
                    throw new InvalidOperationException("Can only resume suspended keys");
                case KeyActionType.Archive when key.Status != KeyStatus.Active && key.Status != KeyStatus.Suspended:
                    throw new InvalidOperationException("Can only archive active or suspended keys");
            }
        }

        private async Task<KeyActionDto> GetKeyActionDtoAsync(int actionId)
        {
            var action = await _context.KeyActions
                .Include(ka => ka.Votes)
                .ThenInclude(v => v.TeamMember)
                .Include(ka => ka.Initiator)
                .FirstOrDefaultAsync(ka => ka.Id == actionId);

            if (action == null)
                throw new KeyNotFoundException($"Key action with ID {actionId} not found");

            return new KeyActionDto
            {
                Id = action.Id,
                Type = action.Type,
                Status = action.Status,
                RequestDate = action.RequestDate,
                Reason = action.Reason,
                RequiresQuorum = action.RequiresQuorum,
                RequiredVotes = action.RequiredVotes,
                CurrentVotes = action.Votes.Count,
                IsEmergency = action.IsEmergency,
                InitiatorName = action.Initiator.Name,
                Votes = action.Votes.Select(v => new KeyActionVoteDto
                {
                    VoterName = v.TeamMember.Name,
                    Approved = v.Approved,
                    VoteDate = v.VoteDate,
                    UsedMFA = v.WasMFAUsed
                }).ToList()
            };
        }

        private string GenerateKeyVaultUri(string keyName)
        {
            // TODO: Implement actual Key Vault URI generation logic
            return $"https://keyvault.azure.net/keys/{keyName.ToLower()}-{DateTime.UtcNow.Ticks}";
        }

        private async Task<CryptoKeyDto> GetKeyDtoAsync(int keyId)
        {
            var key = await _context.CryptoKeys
                .Include(k => k.ManagingTeam)
                .FirstOrDefaultAsync(k => k.Id == keyId);

            if (key == null)
                throw new KeyNotFoundException($"Key with ID {keyId} not found");

            return new CryptoKeyDto
            {
                Id = key.Id,
                Name = key.Name,
                Description = key.Description,
                Type = key.Type,
                Status = key.Status,
                Version = key.Version,
                Algorithm = key.Algorithm,
                KeySizeInBits = key.KeySizeInBits,
                CreateDate = key.CreateDate,
                ActivationDate = key.ActivationDate,
                ExpiryDate = key.ExpiryDate,
                LastRotationDate = key.LastRotationDate,
                NextRotationDate = key.NextRotationDate,
                RequiresDoubleAuth = key.RequiresDoubleAuth,
                IsHSMBacked = key.IsHSMBacked,
                UsageCount = key.UsageCount,
                ManagingTeamName = key.ManagingTeam.Name
            };
        }

        // TODO: Implement these methods from user context
        private int GetCurrentUserId() => 1; // Placeholder
        private string GetCurrentIpAddress() => "127.0.0.1"; // Placeholder
        private string GetCurrentUserAgent() => "CryptAply/1.0"; // Placeholder

        // Placeholder methods for key state changes - to be implemented with actual key operations
        private async Task ActivateKeyAsync(int keyId) { }
        private async Task SuspendKeyAsync(int keyId) { }
        private async Task ResumeKeyAsync(int keyId) { }
        private async Task ArchiveKeyAsync(int keyId) { }

        public async Task<KeyMetricsDto> GetKeyMetricsAsync(int keyId)
        {
            var key = await _context.CryptoKeys
                .Include(k => k.UsageLogs)
                .FirstOrDefaultAsync(k => k.Id == keyId);

            if (key == null)
                throw new KeyNotFoundException($"Key with ID {keyId} not found");

            var pendingActionsCount = await _context.KeyActions
                .CountAsync(ka => ka.CryptoKeyId == keyId && ka.Status == KeyActionStatus.Pending);

            var recentUsage = await _context.KeyUsageLogs
                .Where(l => l.CryptoKeyId == keyId)
                .OrderByDescending(l => l.Timestamp)
                .Take(10)
                .Select(l => new KeyUsageLogDto
                {
                    Timestamp = l.Timestamp,
                    Operation = l.Operation,
                    Application = l.Application,
                    WasSuccessful = l.WasSuccessful,
                    ErrorMessage = l.ErrorMessage
                })
                .ToListAsync();

            var daysUntilExpiry = key.ExpiryDate.HasValue 
                ? (int)(key.ExpiryDate.Value - DateTime.UtcNow).TotalDays 
                : -1;

            return new KeyMetricsDto
            {
                TotalUsageCount = key.UsageCount,
                LastUsed = key.LastUsedDate,
                PendingActionsCount = pendingActionsCount,
                NextScheduledRotation = key.NextRotationDate,
                DaysUntilExpiry = daysUntilExpiry,
                RecentUsage = recentUsage
            };
        }

        public async Task<IEnumerable<KeyUsageLogDto>> GetKeyUsageHistoryAsync(
            int keyId, DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Start date must be before end date");

            var logs = await _context.KeyUsageLogs
                .Where(l => l.CryptoKeyId == keyId && 
                           l.Timestamp >= startDate && 
                           l.Timestamp <= endDate)
                .OrderByDescending(l => l.Timestamp)
                .Select(l => new KeyUsageLogDto
                {
                    Timestamp = l.Timestamp,
                    Operation = l.Operation,
                    Application = l.Application,
                    WasSuccessful = l.WasSuccessful,
                    ErrorMessage = l.ErrorMessage
                })
                .ToListAsync();

            return logs;
        }

        public async Task<bool> ValidateKeyUsageAsync(int keyId, ValidateKeyUsageRequest request)
        {
            var key = await _context.CryptoKeys
                .Include(k => k.ManagingTeam)
                .FirstOrDefaultAsync(k => k.Id == keyId);

            if (key == null)
                throw new KeyNotFoundException($"Key with ID {keyId} not found");

            // Validate key status
            if (key.Status != KeyStatus.Active)
            {
                await LogKeyUsageAsync(key.Id, request.Operation, request.Application, false, 
                    $"Key is not active (Current status: {key.Status})");
                return false;
            }

            // Check expiry
            if (key.ExpiryDate.HasValue && key.ExpiryDate.Value <= DateTime.UtcNow)
            {
                await LogKeyUsageAsync(key.Id, request.Operation, request.Application, false, 
                    "Key has expired");
                return false;
            }

            // Validate MFA requirement
            if (key.RequiresDoubleAuth && !request.RequiresMFA)
            {
                await LogKeyUsageAsync(key.Id, request.Operation, request.Application, false, 
                    "MFA is required for this key");
                return false;
            }

            // Log successful validation
            await LogKeyUsageAsync(key.Id, request.Operation, request.Application, true, null);

            // Update usage statistics
            key.UsageCount++;
            key.LastUsedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task LogKeyUsageAsync(int keyId, string operation, string application, 
            bool wasSuccessful, string errorMessage = null)
        {
            var log = new KeyUsageLog
            {
                CryptoKeyId = keyId,
                Timestamp = DateTime.UtcNow,
                Operation = operation,
                UserId = GetCurrentUserId().ToString(),
                Application = application,
                IpAddress = GetCurrentIpAddress(),
                WasSuccessful = wasSuccessful,
                ErrorMessage = errorMessage,
                AdditionalData = GetUsageContext()
            };

            _context.KeyUsageLogs.Add(log);
            await _context.SaveChangesAsync();

            if (!wasSuccessful)
            {
                _logger.LogWarning("Failed key usage attempt: Key {KeyId}, Operation {Operation}, Error: {Error}",
                    keyId, operation, errorMessage);
            }
        }

        private string GetUsageContext()
        {
            // TODO: Implement to gather relevant context (e.g., thread info, correlation ID, etc.)
            return null;
        }

        public async Task<UsageStatisticsDto> GetUsageStatisticsAsync(int keyId, DateTime startDate, DateTime endDate)
        {
            var logs = await _context.KeyUsageLogs
                .Where(l => l.CryptoKeyId == keyId && 
                           l.Timestamp >= startDate && 
                           l.Timestamp <= endDate)
                .ToListAsync();

            return new UsageStatisticsDto
            {
                TotalOperations = logs.Count,
                SuccessfulOperations = logs.Count(l => l.WasSuccessful),
                FailedOperations = logs.Count(l => !l.WasSuccessful),
                OperationBreakdown = logs.GroupBy(l => l.Operation)
                    .ToDictionary(g => g.Key, g => g.Count()),
                ApplicationBreakdown = logs.GroupBy(l => l.Application)
                    .ToDictionary(g => g.Key, g => g.Count()),
                ErrorBreakdown = logs.Where(l => !l.WasSuccessful)
                    .GroupBy(l => l.ErrorMessage)
                    .ToDictionary(g => g.Key, g => g.Count()),
                UsageByHour = logs.GroupBy(l => l.Timestamp.Hour)
                    .ToDictionary(g => g.Key, g => g.Count()),
                SuccessRate = logs.Any() 
                    ? (double)logs.Count(l => l.WasSuccessful) / logs.Count * 100 
                    : 0
            };
        }

        private class UsageStatisticsDto
        {
            public int TotalOperations { get; set; }
            public int SuccessfulOperations { get; set; }
            public int FailedOperations { get; set; }
            public Dictionary<string, int> OperationBreakdown { get; set; }
            public Dictionary<string, int> ApplicationBreakdown { get; set; }
            public Dictionary<string, int> ErrorBreakdown { get; set; }
            public Dictionary<int, int> UsageByHour { get; set; }
            public double SuccessRate { get; set; }
        }
    }
}
