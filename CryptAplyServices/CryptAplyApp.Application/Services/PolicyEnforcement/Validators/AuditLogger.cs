using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Services.PolicyEnforcement.Validators
{
    public class AuditLogger : IAuditLogger
    {
        private readonly ILogger<AuditLogger> _logger;
        private readonly Dictionary<string, byte[]> _hashChain;
        private readonly object _lockObject = new object();

        public AuditLogger(ILogger<AuditLogger> logger)
        {
            _logger = logger;
            _hashChain = new Dictionary<string, byte[]>();
        }

        public async Task LogAsync(AuditEvent auditEvent)
        {
            try
            {
                // Add chain hash to ensure log integrity
                var chainHash = await CreateChainHashAsync(auditEvent);
                auditEvent.Metadata ??= new Dictionary<string, string>();
                auditEvent.Metadata["ChainHash"] = Convert.ToBase64String(chainHash);

                // Serialize and store the event
                var serializedEvent = JsonSerializer.Serialize(auditEvent, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                // In production, this would write to a secure audit store
                // For now, we just log it
                _logger.LogInformation("Audit Event: {AuditEvent}", serializedEvent);

                // Update hash chain
                lock (_lockObject)
                {
                    _hashChain[auditEvent.Timestamp.ToString("yyyyMMddHH")] = chainHash;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log audit event for {ApplicationId}", auditEvent.ApplicationId);
                throw;
            }
        }

        public async Task<AuditEvent[]> GetAuditTrailAsync(string keyId, string timeRange)
        {
            try
            {
                // In production, this would query the secure audit store
                // For now, return empty array
                _logger.LogInformation("Retrieving audit trail for key {KeyId} in time range {TimeRange}", 
                    keyId, timeRange);
                return Array.Empty<AuditEvent>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve audit trail for key {KeyId}", keyId);
                throw;
            }
        }

        public async Task<bool> VerifyAuditLogIntegrityAsync(string timeRange)
        {
            try
            {
                // In production, this would verify the hash chain across all logs
                // For now, we just verify the hash chain we have in memory
                var previousHash = Array.Empty<byte>();
                var verified = true;

                lock (_lockObject)
                {
                    foreach (var hourlyHash in _hashChain.Values)
                    {
                        if (previousHash.Length > 0)
                        {
                            // Verify chain continuity
                            using var sha256 = SHA256.Create();
                            var combinedHash = sha256.ComputeHash(
                                Combine(previousHash, hourlyHash));
                            
                            if (!AreHashesEqual(combinedHash, hourlyHash))
                            {
                                verified = false;
                                break;
                            }
                        }
                        previousHash = hourlyHash;
                    }
                }

                _logger.LogInformation("Audit log integrity verification result: {Result}", verified);
                return verified;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to verify audit log integrity");
                return false;
            }
        }

        private async Task<byte[]> CreateChainHashAsync(AuditEvent auditEvent)
        {
            var hourKey = auditEvent.Timestamp.ToString("yyyyMMddHH");
            byte[] previousHash;

            lock (_lockObject)
            {
                _hashChain.TryGetValue(hourKey, out previousHash);
            }

            using var sha256 = SHA256.Create();
            var eventBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(auditEvent));
            var eventHash = sha256.ComputeHash(eventBytes);

            if (previousHash != null)
            {
                return sha256.ComputeHash(Combine(previousHash, eventHash));
            }

            return eventHash;
        }

        private byte[] Combine(byte[] first, byte[] second)
        {
            var combined = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, combined, 0, first.Length);
            Buffer.BlockCopy(second, 0, combined, first.Length, second.Length);
            return combined;
        }

        private bool AreHashesEqual(byte[] first, byte[] second)
        {
            if (first.Length != second.Length)
            {
                return false;
            }

            for (var i = 0; i < first.Length; i++)
            {
                if (first[i] != second[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
