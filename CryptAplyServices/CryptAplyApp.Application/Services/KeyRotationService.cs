using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Infrastructure.Repositories;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Services
{
    public class KeyRotationService
    {
        private readonly ILogger<KeyRotationService> _logger;
        private readonly IKeyRepository _keyRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly AlertingService _alertingService;
        private readonly IKeyActionVoteRepository _voteRepository;

        public KeyRotationService(
            ILogger<KeyRotationService> logger,
            IKeyRepository keyRepository,
            IEncryptionService encryptionService,
            AlertingService alertingService,
            IKeyActionVoteRepository voteRepository)
        {
            _logger = logger;
            _keyRepository = keyRepository;
            _encryptionService = encryptionService;
            _alertingService = alertingService;
            _voteRepository = voteRepository;
        }

        public async Task<RotationResult> InitiateKeyRotationAsync(string keyId, string initiatedBy)
        {
            try
            {
                var existingKey = await _keyRepository.GetKeyByIdAsync(keyId);
                if (existingKey == null)
                {
                    throw new KeyNotFoundException($"Key {keyId} not found");
                }

                // Check if rotation is already in progress
                if (existingKey.RotationStatus == KeyRotationStatus.InProgress)
                {
                    return new RotationResult
                    {
                        Success = false,
                        Message = "Key rotation is already in progress"
                    };
                }

                // Create new key version
                var newKeyVersion = await _encryptionService.GenerateKeyAsync();
                
                // Create rotation record
                var rotation = new KeyRotation
                {
                    KeyId = keyId,
                    OldVersion = existingKey.Version,
                    NewVersion = existingKey.Version + 1,
                    InitiatedBy = initiatedBy,
                    InitiatedAt = DateTime.UtcNow,
                    Status = KeyRotationStatus.InProgress,
                    NewKeyMaterial = newKeyVersion.KeyMaterial
                };

                await _keyRepository.CreateKeyRotationAsync(rotation);

                // Update key status
                existingKey.RotationStatus = KeyRotationStatus.InProgress;
                await _keyRepository.UpdateKeyAsync(existingKey);

                // Create approval request
                await CreateRotationApprovalRequestAsync(rotation);

                // Send alert
                await _alertingService.SendAlertAsync(new Alert
                {
                    Title = "Key Rotation Initiated",
                    Description = $"Key rotation initiated for key {keyId}",
                    Type = AlertType.KeyRotation,
                    Severity = AlertSeverity.High,
                    Category = "Key Management",
                    Tags = new List<string> { "key-rotation", "security" },
                    Metadata = new Dictionary<string, string>
                    {
                        { "KeyId", keyId },
                        { "InitiatedBy", initiatedBy },
                        { "OldVersion", existingKey.Version.ToString() },
                        { "NewVersion", (existingKey.Version + 1).ToString() }
                    }
                });

                return new RotationResult
                {
                    Success = true,
                    Message = "Key rotation initiated successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating key rotation for key {KeyId}", keyId);
                throw;
            }
        }

        public async Task<ApprovalResult> ApproveRotationAsync(string keyId, string approver)
        {
            try
            {
                var rotation = await _keyRepository.GetActiveRotationAsync(keyId);
                if (rotation == null)
                {
                    throw new InvalidOperationException($"No active rotation found for key {keyId}");
                }

                // Record vote
                await _voteRepository.CreateVoteAsync(new KeyActionVote
                {
                    KeyId = keyId,
                    ActionType = KeyActionType.Rotation,
                    Voter = approver,
                    Vote = VoteType.Approve,
                    Timestamp = DateTime.UtcNow,
                    RotationId = rotation.RotationId
                });

                // Check if we have enough approvals
                var votes = await _voteRepository.GetVotesForRotationAsync(rotation.RotationId);
                if (votes.Count >= 2) // Requiring 2 approvals
                {
                    return await CompleteRotationAsync(rotation);
                }

                return new ApprovalResult
                {
                    Success = true,
                    Message = "Approval recorded, waiting for additional approvals"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving rotation for key {KeyId}", keyId);
                throw;
            }
        }

        private async Task<ApprovalResult> CompleteRotationAsync(KeyRotation rotation)
        {
            try
            {
                // Get the key
                var key = await _keyRepository.GetKeyByIdAsync(rotation.KeyId);

                // Archive old version
                await _keyRepository.ArchiveKeyVersionAsync(key.Id, key.Version, key.KeyMaterial);

                // Update key with new version
                key.Version = rotation.NewVersion;
                key.KeyMaterial = rotation.NewKeyMaterial;
                key.LastRotatedAt = DateTime.UtcNow;
                key.RotationStatus = KeyRotationStatus.Completed;

                await _keyRepository.UpdateKeyAsync(key);

                // Update rotation status
                rotation.Status = KeyRotationStatus.Completed;
                rotation.CompletedAt = DateTime.UtcNow;
                await _keyRepository.UpdateKeyRotationAsync(rotation);

                // Send alert
                await _alertingService.SendAlertAsync(new Alert
                {
                    Title = "Key Rotation Completed",
                    Description = $"Key rotation completed for key {rotation.KeyId}",
                    Type = AlertType.KeyRotation,
                    Severity = AlertSeverity.High,
                    Category = "Key Management",
                    Tags = new List<string> { "key-rotation", "security" },
                    Metadata = new Dictionary<string, string>
                    {
                        { "KeyId", rotation.KeyId },
                        { "OldVersion", rotation.OldVersion.ToString() },
                        { "NewVersion", rotation.NewVersion.ToString() }
                    }
                });

                return new ApprovalResult
                {
                    Success = true,
                    Message = "Key rotation completed successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing rotation for key {KeyId}", rotation.KeyId);
                throw;
            }
        }

        private async Task CreateRotationApprovalRequestAsync(KeyRotation rotation)
        {
            await _alertingService.SendAlertAsync(new Alert
            {
                Title = "Key Rotation Approval Required",
                Description = $"Approval required for key rotation of key {rotation.KeyId}",
                Type = AlertType.PendingActions,
                Severity = AlertSeverity.High,
                Category = "Key Management",
                Tags = new List<string> { "key-rotation", "approval-required" },
                Metadata = new Dictionary<string, string>
                {
                    { "KeyId", rotation.KeyId },
                    { "InitiatedBy", rotation.InitiatedBy },
                    { "OldVersion", rotation.OldVersion.ToString() },
                    { "NewVersion", rotation.NewVersion.ToString() }
                }
            });
        }
    }

    public class RotationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class ApprovalResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
