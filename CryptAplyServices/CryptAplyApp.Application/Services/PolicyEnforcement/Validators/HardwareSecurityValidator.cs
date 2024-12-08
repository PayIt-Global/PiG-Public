using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Services.PolicyEnforcement.Validators
{
    public class HardwareSecurityValidator : IHardwareSecurityValidator
    {
        private readonly ILogger<HardwareSecurityValidator> _logger;
        private readonly IAuditLogger _auditLogger;
        private readonly IAlertingService _alertingService;
        private readonly IHSMService _hsmService;
        private readonly ITpmService _tpmService;
        private readonly ISecureEnclaveService _secureEnclaveService;

        public HardwareSecurityValidator(
            ILogger<HardwareSecurityValidator> logger,
            IAuditLogger auditLogger,
            IAlertingService alertingService,
            IHSMService hsmService,
            ITpmService tpmService,
            ISecureEnclaveService secureEnclaveService)
        {
            _logger = logger;
            _auditLogger = auditLogger;
            _alertingService = alertingService;
            _hsmService = hsmService;
            _tpmService = tpmService;
            _secureEnclaveService = secureEnclaveService;
        }

        public async Task<bool> ValidateHardwareSecurityAsync(KeyOperationContext context)
        {
            try
            {
                // Check HSM availability and health
                if (!await ValidateHSMStatusAsync(context))
                {
                    await LogHardwareSecurityViolationAsync(context, "HSM validation failed");
                    return false;
                }

                // Validate TPM if required
                if (context.RequiresTPM && !await ValidateTPMStatusAsync(context))
                {
                    await LogHardwareSecurityViolationAsync(context, "TPM validation failed");
                    return false;
                }

                // Validate Secure Enclave if required
                if (context.RequiresSecureEnclave && !await ValidateSecureEnclaveStatusAsync(context))
                {
                    await LogHardwareSecurityViolationAsync(context, "Secure Enclave validation failed");
                    return false;
                }

                // Validate hardware-based key attributes
                if (!await ValidateKeyAttributesAsync(context))
                {
                    await LogHardwareSecurityViolationAsync(context, "Key attributes validation failed");
                    return false;
                }

                // Validate hardware security boundaries
                if (!await ValidateSecurityBoundariesAsync(context))
                {
                    await LogHardwareSecurityViolationAsync(context, "Security boundary validation failed");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating hardware security for {ApplicationId}", context.ApplicationId);
                return false;
            }
        }

        private async Task<bool> ValidateHSMStatusAsync(KeyOperationContext context)
        {
            var hsmStatus = await _hsmService.GetHSMStatusAsync();
            
            if (!hsmStatus.IsAvailable)
            {
                _logger.LogError("HSM is not available for {ApplicationId}", context.ApplicationId);
                return false;
            }

            if (!hsmStatus.IsHealthy)
            {
                _logger.LogError("HSM health check failed for {ApplicationId}", context.ApplicationId);
                return false;
            }

            // Validate HSM quorum requirements
            if (!await ValidateHSMQuorumAsync(context))
            {
                _logger.LogError("HSM quorum validation failed for {ApplicationId}", context.ApplicationId);
                return false;
            }

            // Validate HSM firmware version
            if (!ValidateHSMFirmware(hsmStatus))
            {
                _logger.LogError("HSM firmware validation failed for {ApplicationId}", context.ApplicationId);
                return false;
            }

            return true;
        }

        private async Task<bool> ValidateTPMStatusAsync(KeyOperationContext context)
        {
            var tpmStatus = await _tpmService.GetTPMStatusAsync();

            if (!tpmStatus.IsAvailable)
            {
                _logger.LogError("TPM is not available for {ApplicationId}", context.ApplicationId);
                return false;
            }

            if (!tpmStatus.IsHealthy)
            {
                _logger.LogError("TPM health check failed for {ApplicationId}", context.ApplicationId);
                return false;
            }

            // Validate TPM PCR values
            if (!await ValidateTPMPCRValuesAsync(context))
            {
                _logger.LogError("TPM PCR validation failed for {ApplicationId}", context.ApplicationId);
                return false;
            }

            return true;
        }

        private async Task<bool> ValidateSecureEnclaveStatusAsync(KeyOperationContext context)
        {
            var enclaveStatus = await _secureEnclaveService.GetStatusAsync();

            if (!enclaveStatus.IsAvailable)
            {
                _logger.LogError("Secure Enclave is not available for {ApplicationId}", context.ApplicationId);
                return false;
            }

            if (!enclaveStatus.IsHealthy)
            {
                _logger.LogError("Secure Enclave health check failed for {ApplicationId}", context.ApplicationId);
                return false;
            }

            // Validate enclave attestation
            if (!await ValidateEnclaveAttestationAsync(context))
            {
                _logger.LogError("Secure Enclave attestation failed for {ApplicationId}", context.ApplicationId);
                return false;
            }

            return true;
        }

        private async Task<bool> ValidateKeyAttributesAsync(KeyOperationContext context)
        {
            // Validate key is non-exportable
            if (!await ValidateNonExportableKeyAsync(context))
                return false;

            // Validate key usage restrictions
            if (!await ValidateKeyUsageRestrictionsAsync(context))
                return false;

            // Validate key backup protection
            if (!await ValidateKeyBackupProtectionAsync(context))
                return false;

            return true;
        }

        private async Task<bool> ValidateSecurityBoundariesAsync(KeyOperationContext context)
        {
            // Validate physical security boundaries
            if (!await ValidatePhysicalSecurityAsync(context))
                return false;

            // Validate logical security boundaries
            if (!await ValidateLogicalSecurityAsync(context))
                return false;

            // Validate cryptographic boundaries
            if (!await ValidateCryptographicBoundariesAsync(context))
                return false;

            return true;
        }

        private async Task<bool> ValidateHSMQuorumAsync(KeyOperationContext context)
        {
            var quorumStatus = await _hsmService.GetQuorumStatusAsync();
            return quorumStatus.IsQuorumMet && quorumStatus.ActiveOperators >= quorumStatus.RequiredOperators;
        }

        private bool ValidateHSMFirmware(HSMStatus status)
        {
            return Version.Parse(status.FirmwareVersion) >= Version.Parse(status.MinimumRequiredVersion);
        }

        private async Task<bool> ValidateTPMPCRValuesAsync(KeyOperationContext context)
        {
            var pcrValues = await _tpmService.GetPCRValuesAsync();
            var expectedValues = await _tpmService.GetExpectedPCRValuesAsync();

            return pcrValues.SequenceEqual(expectedValues);
        }

        private async Task<bool> ValidateEnclaveAttestationAsync(KeyOperationContext context)
        {
            var attestation = await _secureEnclaveService.GetAttestationAsync();
            return await _secureEnclaveService.VerifyAttestationAsync(attestation);
        }

        private async Task<bool> ValidateNonExportableKeyAsync(KeyOperationContext context)
        {
            var keyAttributes = await _hsmService.GetKeyAttributesAsync(context.KeyId);
            return keyAttributes.IsNonExportable;
        }

        private async Task<bool> ValidateKeyUsageRestrictionsAsync(KeyOperationContext context)
        {
            var keyAttributes = await _hsmService.GetKeyAttributesAsync(context.KeyId);
            return keyAttributes.AllowedOperations.Contains(context.Operation);
        }

        private async Task<bool> ValidateKeyBackupProtectionAsync(KeyOperationContext context)
        {
            var backupStatus = await _hsmService.GetKeyBackupStatusAsync(context.KeyId);
            return backupStatus.IsProtected && backupStatus.LastBackupTime > DateTime.UtcNow.AddDays(-30);
        }

        private async Task<bool> ValidatePhysicalSecurityAsync(KeyOperationContext context)
        {
            var physicalStatus = await _hsmService.GetPhysicalSecurityStatusAsync();
            return physicalStatus.IsIntact && !physicalStatus.HasTamperEvents;
        }

        private async Task<bool> ValidateLogicalSecurityAsync(KeyOperationContext context)
        {
            var logicalStatus = await _hsmService.GetLogicalSecurityStatusAsync();
            return logicalStatus.IsSecure && logicalStatus.SecurityLevel >= context.RequiredSecurityLevel;
        }

        private async Task<bool> ValidateCryptographicBoundariesAsync(KeyOperationContext context)
        {
            var boundaryStatus = await _hsmService.GetCryptographicBoundaryStatusAsync();
            return boundaryStatus.IsIntact && boundaryStatus.CompliesWith.Contains("FIPS 140-2");
        }

        private async Task LogHardwareSecurityViolationAsync(
            KeyOperationContext context,
            string reason)
        {
            await _auditLogger.LogAsync(new AuditEvent
            {
                EventType = "HardwareSecurityViolation",
                ApplicationId = context.ApplicationId,
                Success = false,
                Timestamp = DateTime.UtcNow,
                Details = new[] { reason },
                Metadata = new Dictionary<string, string>
                {
                    { "Reason", reason },
                    { "Operation", context.Operation },
                    { "KeyId", context.KeyId }
                }
            });

            await _alertingService.RaiseAlertAsync(new Alert
            {
                Severity = AlertSeverity.Critical,
                Source = "HardwareSecurityValidator",
                Message = $"Hardware security violation: {reason}",
                Timestamp = DateTime.UtcNow,
                Details = $"Application: {context.ApplicationId}, Operation: {context.Operation}"
            });
        }
    }
}
