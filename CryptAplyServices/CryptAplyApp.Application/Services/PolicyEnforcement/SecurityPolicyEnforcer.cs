using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Application.Models;
using CryptAplyApp.Application.Exceptions;

namespace CryptAplyApp.Application.Services.PolicyEnforcement
{
    public interface ISecurityPolicyEnforcer
    {
        Task<PolicyValidationResult> ValidateKeyOperationAsync(KeyOperationContext context);
        Task<PolicyValidationResult> ValidateKeyCreationAsync(KeyCreationContext context);
        Task<PolicyValidationResult> ValidateKeyRotationAsync(KeyRotationContext context);
        Task LogPolicyValidationAsync(PolicyValidationResult result);
    }

    public class SecurityPolicyEnforcer : ISecurityPolicyEnforcer
    {
        private readonly ILogger<SecurityPolicyEnforcer> _logger;
        private readonly Dictionary<string, SecurityPolicy> _applicationPolicies;
        private readonly IAuditLogger _auditLogger;
        private readonly ITimeWindowValidator _timeValidator;
        private readonly IQuotaValidator _quotaValidator;
        private readonly INetworkValidator _networkValidator;

        public SecurityPolicyEnforcer(
            ILogger<SecurityPolicyEnforcer> logger,
            IAuditLogger auditLogger,
            ITimeWindowValidator timeValidator,
            IQuotaValidator quotaValidator,
            INetworkValidator networkValidator)
        {
            _logger = logger;
            _auditLogger = auditLogger;
            _timeValidator = timeValidator;
            _quotaValidator = quotaValidator;
            _networkValidator = networkValidator;
            _applicationPolicies = new Dictionary<string, SecurityPolicy>();
        }

        public async Task<PolicyValidationResult> ValidateKeyOperationAsync(KeyOperationContext context)
        {
            var policy = GetPolicyForApplication(context.ApplicationId);
            var result = new PolicyValidationResult { IsValid = true };

            try
            {
                // Context Validation
                await ValidateContextAsync(context, policy.ContextControls);

                // Time Window Validation
                if (!await _timeValidator.IsWithinAllowedWindowAsync(context, policy.TimeControls))
                {
                    result.AddViolation("Operation not allowed during this time window");
                }

                // Quota Validation
                if (!await _quotaValidator.CheckQuotasAsync(context, policy.QuotaControls))
                {
                    result.AddViolation("Operation would exceed quota limits");
                }

                // Purpose Validation
                if (!ValidateKeyPurpose(context, policy.PurposeControls))
                {
                    result.AddViolation("Operation not allowed for this key purpose");
                }

                // Export Control Validation
                if (IsExportOperation(context.Operation))
                {
                    ValidateExportOperation(context, policy.ExportControls, result);
                }

                // HSM Validation
                if (!ValidateHsmRequirements(context, policy.HsmControls))
                {
                    result.AddViolation("Operation does not meet HSM requirements");
                }

                // Application-Specific Validation
                if (!ValidateApplicationPermissions(context, policy.ApplicationControls))
                {
                    result.AddViolation("Application does not have permission for this operation");
                }

                // Compliance Validation
                await ValidateComplianceRequirementsAsync(context, policy.ComplianceControls, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during policy validation for {ApplicationId}", context.ApplicationId);
                result.AddViolation($"Policy validation error: {ex.Message}");
            }

            await LogPolicyValidationAsync(result);
            return result;
        }

        public async Task<PolicyValidationResult> ValidateKeyCreationAsync(KeyCreationContext context)
        {
            var policy = GetPolicyForApplication(context.ApplicationId);
            var result = new PolicyValidationResult { IsValid = true };

            try
            {
                // Validate HSM Requirements
                if (policy.HsmControls.RequireHsmBacked && !context.IsHsmBacked)
                {
                    result.AddViolation("Keys must be HSM-backed");
                }

                // Validate Key Properties
                if (!ValidateKeyProperties(context, policy.HsmControls.KeyProperties))
                {
                    result.AddViolation("Key properties do not meet policy requirements");
                }

                // Validate Approval Requirements
                if (!await ValidateApprovalRequirementsAsync(context, policy.ApprovalControls))
                {
                    result.AddViolation("Required approvals not obtained");
                }

                // Validate Compliance Requirements
                await ValidateComplianceRequirementsAsync(context, policy.ComplianceControls, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during key creation validation for {ApplicationId}", context.ApplicationId);
                result.AddViolation($"Key creation validation error: {ex.Message}");
            }

            await LogPolicyValidationAsync(result);
            return result;
        }

        public async Task<PolicyValidationResult> ValidateKeyRotationAsync(KeyRotationContext context)
        {
            var policy = GetPolicyForApplication(context.ApplicationId);
            var result = new PolicyValidationResult { IsValid = true };

            try
            {
                // Validate Time Window
                if (!await _timeValidator.IsWithinAllowedWindowAsync(context, policy.TimeControls))
                {
                    if (!await ValidateEmergencyOverrideAsync(context, policy.TimeControls.EmergencyOverride))
                    {
                        result.AddViolation("Key rotation not allowed during this time window");
                    }
                }

                // Validate Approvals
                if (!await ValidateApprovalRequirementsAsync(context, policy.ApprovalControls))
                {
                    result.AddViolation("Required approvals for key rotation not obtained");
                }

                // Validate HSM Requirements for New Key
                if (!ValidateHsmRequirements(context, policy.HsmControls))
                {
                    result.AddViolation("New key version does not meet HSM requirements");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during key rotation validation for {ApplicationId}", context.ApplicationId);
                result.AddViolation($"Key rotation validation error: {ex.Message}");
            }

            await LogPolicyValidationAsync(result);
            return result;
        }

        public async Task LogPolicyValidationAsync(PolicyValidationResult result)
        {
            await _auditLogger.LogAsync(new AuditEvent
            {
                EventType = "PolicyValidation",
                Success = result.IsValid,
                Details = result.Violations,
                Timestamp = DateTime.UtcNow
            });
        }

        private SecurityPolicy GetPolicyForApplication(string applicationId)
        {
            if (!_applicationPolicies.TryGetValue(applicationId, out var policy))
            {
                throw new PolicyNotFoundException($"No security policy found for application {applicationId}");
            }
            return policy;
        }

        private async Task ValidateContextAsync(KeyOperationContext context, ContextBasedAccess controls)
        {
            // Network Validation
            if (!await _networkValidator.ValidateNetworkAccessAsync(context, controls.NetworkControls))
            {
                throw new PolicyViolationException("Network access requirements not met");
            }

            // Label Validation
            if (controls.RequiredLabels != null)
            {
                foreach (var requiredLabel in controls.RequiredLabels)
                {
                    if (!context.Labels.TryGetValue(requiredLabel.Key, out var value) ||
                        !requiredLabel.Value.Contains(value))
                    {
                        throw new PolicyViolationException($"Required label {requiredLabel.Key} not present or invalid");
                    }
                }
            }

            // Workload Identity Validation
            if (controls.AllowedWorkloadIdentities != null &&
                !controls.AllowedWorkloadIdentities.Contains(context.WorkloadIdentity))
            {
                throw new PolicyViolationException("Workload identity not allowed");
            }
        }

        private bool ValidateKeyPurpose(KeyOperationContext context, KeyPurposePolicy controls)
        {
            return controls.AllowedOperations.Contains(context.Operation) &&
                   (!controls.AllowedDataTypes.Any() || controls.AllowedDataTypes.Contains(context.DataType));
        }

        private void ValidateExportOperation(KeyOperationContext context, KeyExportPolicy controls, PolicyValidationResult result)
        {
            if (!controls.AllowExport)
            {
                result.AddViolation("Key export operations are not allowed");
                return;
            }

            if (controls.Restrictions.RequireKeyWrapping && !context.IsWrapped)
            {
                result.AddViolation("Key must be wrapped for export");
            }

            if (!controls.Restrictions.AllowPlaintextExport && context.IsPlaintext)
            {
                result.AddViolation("Plaintext key export not allowed");
            }
        }

        private bool ValidateHsmRequirements(KeyOperationContext context, HardwareSecurityPolicy controls)
        {
            return !controls.RequireHsmBacked || context.IsHsmBacked;
        }

        private bool ValidateKeyProperties(KeyCreationContext context, HsmKeyProperties controls)
        {
            return controls.AllowedKeyOrigins.Contains(context.KeyOrigin) &&
                   int.Parse(context.KeySize) >= int.Parse(controls.MinimumKeySize);
        }

        private bool ValidateApplicationPermissions(KeyOperationContext context, ApplicationPolicy controls)
        {
            return controls.AllowedOperationsByApp.TryGetValue(context.ApplicationId, out var allowedOps) &&
                   allowedOps.Contains(context.Operation);
        }

        private async Task ValidateComplianceRequirementsAsync(KeyOperationContext context, CompliancePolicy controls, PolicyValidationResult result)
        {
            if (controls.RequiredClassifications.Any(c => !context.Classifications.Contains(c)))
            {
                result.AddViolation("Required data classifications not met");
            }

            if (controls.AuditControls.RequireAuditLog)
            {
                await _auditLogger.LogAsync(new AuditEvent
                {
                    EventType = "KeyOperation",
                    KeyId = context.KeyId,
                    Operation = context.Operation,
                    ApplicationId = context.ApplicationId,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        private async Task<bool> ValidateApprovalRequirementsAsync(KeyOperationContext context, ApprovalPolicy controls)
        {
            // Implementation would check approval system
            return true; // Placeholder
        }

        private async Task<bool> ValidateEmergencyOverrideAsync(KeyOperationContext context, EmergencyAccess controls)
        {
            // Implementation would check emergency override system
            return false; // Placeholder
        }

        private bool IsExportOperation(string operation)
        {
            return operation.Equals("export", StringComparison.OrdinalIgnoreCase) ||
                   operation.Equals("backup", StringComparison.OrdinalIgnoreCase);
        }
    }

    public class PolicyValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Violations { get; } = new List<string>();

        public void AddViolation(string violation)
        {
            IsValid = false;
            Violations.Add(violation);
        }
    }

    public class PolicyViolationException : Exception
    {
        public PolicyViolationException(string message) : base(message) { }
    }

    public class PolicyNotFoundException : Exception
    {
        public PolicyNotFoundException(string message) : base(message) { }
    }
}
