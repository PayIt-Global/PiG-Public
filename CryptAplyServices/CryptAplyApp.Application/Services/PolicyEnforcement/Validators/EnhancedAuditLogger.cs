using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Services.PolicyEnforcement.Validators
{
    public class EnhancedAuditLogger : IAuditLogger
    {
        private readonly ILogger<EnhancedAuditLogger> _logger;
        private readonly IBlockchainService _blockchainService;
        private readonly IAIAnalysisService _aiAnalysisService;
        private readonly IAlertingService _alertingService;
        private readonly Queue<AuditEvent> _auditBuffer;
        private readonly object _lockObject = new object();
        private const int BUFFER_SIZE = 100;

        public EnhancedAuditLogger(
            ILogger<EnhancedAuditLogger> logger,
            IBlockchainService blockchainService,
            IAIAnalysisService aiAnalysisService,
            IAlertingService alertingService)
        {
            _logger = logger;
            _blockchainService = blockchainService;
            _aiAnalysisService = aiAnalysisService;
            _alertingService = alertingService;
            _auditBuffer = new Queue<AuditEvent>();
        }

        public async Task LogAsync(AuditEvent auditEvent)
        {
            try
            {
                // Enrich the audit event with additional metadata
                EnrichAuditEvent(auditEvent);

                // Generate tamper-evident hash
                var eventHash = GenerateEventHash(auditEvent);
                auditEvent.IntegrityHash = eventHash;

                // Buffer the event
                BufferAuditEvent(auditEvent);

                // Perform AI analysis
                var anomalyScore = await _aiAnalysisService.AnalyzeEventAsync(auditEvent);
                if (anomalyScore > 0.8) // High anomaly score
                {
                    await _alertingService.RaiseAlertAsync(new Alert
                    {
                        Severity = AlertSeverity.High,
                        Source = "EnhancedAuditLogger",
                        Message = $"High anomaly score detected for event: {auditEvent.EventType}",
                        Timestamp = DateTime.UtcNow,
                        Details = JsonSerializer.Serialize(auditEvent)
                    });
                }

                // If buffer is full or event is critical, anchor to blockchain
                if (_auditBuffer.Count >= BUFFER_SIZE || IsCriticalEvent(auditEvent))
                {
                    await AnchorToBlockchainAsync();
                }

                _logger.LogInformation(
                    "Audit event logged successfully. Type: {EventType}, Hash: {Hash}",
                    auditEvent.EventType,
                    eventHash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging audit event");
                throw;
            }
        }

        private void EnrichAuditEvent(AuditEvent auditEvent)
        {
            auditEvent.Metadata ??= new Dictionary<string, string>();
            auditEvent.Metadata["LoggedAt"] = DateTime.UtcNow.ToString("o");
            auditEvent.Metadata["ProcessId"] = System.Diagnostics.Process.GetCurrentProcess().Id.ToString();
            auditEvent.Metadata["MachineName"] = Environment.MachineName;
        }

        private string GenerateEventHash(AuditEvent auditEvent)
        {
            var eventJson = JsonSerializer.Serialize(auditEvent);
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(eventJson));
            return Convert.ToBase64String(hashBytes);
        }

        private void BufferAuditEvent(AuditEvent auditEvent)
        {
            lock (_lockObject)
            {
                _auditBuffer.Enqueue(auditEvent);
            }
        }

        private async Task AnchorToBlockchainAsync()
        {
            List<AuditEvent> events;
            lock (_lockObject)
            {
                events = new List<AuditEvent>(_auditBuffer);
                _auditBuffer.Clear();
            }

            if (events.Count > 0)
            {
                var merkleRoot = ComputeMerkleRoot(events);
                await _blockchainService.AnchorDataAsync(merkleRoot);
            }
        }

        private string ComputeMerkleRoot(List<AuditEvent> events)
        {
            var hashes = events.Select(e => e.IntegrityHash).ToList();
            while (hashes.Count > 1)
            {
                var newHashes = new List<string>();
                for (int i = 0; i < hashes.Count; i += 2)
                {
                    if (i + 1 < hashes.Count)
                    {
                        var combined = hashes[i] + hashes[i + 1];
                        using var sha256 = SHA256.Create();
                        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
                        newHashes.Add(Convert.ToBase64String(hashBytes));
                    }
                    else
                    {
                        newHashes.Add(hashes[i]);
                    }
                }
                hashes = newHashes;
            }
            return hashes.FirstOrDefault() ?? string.Empty;
        }

        private bool IsCriticalEvent(AuditEvent auditEvent)
        {
            return auditEvent.EventType.Contains("Critical") ||
                   auditEvent.EventType.Contains("Security") ||
                   auditEvent.EventType.Contains("Compliance");
        }
    }
}
