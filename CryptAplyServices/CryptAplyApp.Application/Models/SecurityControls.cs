using System;
using System.Collections.Generic;

namespace CryptAplyApp.Application.Models
{
    public class SecurityPolicy
    {
        public ContextBasedAccess ContextControls { get; set; }
        public TimeWindowControls TimeControls { get; set; }
        public UsageQuotas QuotaControls { get; set; }
        public KeyPurposePolicy PurposeControls { get; set; }
        public ApprovalPolicy ApprovalControls { get; set; }
        public CompliancePolicy ComplianceControls { get; set; }
        public ApplicationPolicy ApplicationControls { get; set; }
        public HardwareSecurityPolicy HsmControls { get; set; }
        public KeyExportPolicy ExportControls { get; set; }
    }

    public class ContextBasedAccess
    {
        public string[] AllowedIpRanges { get; set; }
        public string[] AllowedEnvironments { get; set; }
        public Dictionary<string, string[]> RequiredLabels { get; set; }
        public string[] AllowedWorkloadIdentities { get; set; }
        public Dictionary<string, string[]> RequiredClaims { get; set; }
        public NetworkPolicy NetworkControls { get; set; }
    }

    public class NetworkPolicy
    {
        public bool RequirePrivateEndpoint { get; set; }
        public bool AllowInternetAccess { get; set; }
        public string[] AllowedVnetIds { get; set; }
        public string[] AllowedSubnetIds { get; set; }
        public Dictionary<string, string[]> RequiredNetworkTags { get; set; }
    }

    public class TimeWindowControls
    {
        public TimeSpan[] AllowedWindows { get; set; }
        public string[] AllowedDays { get; set; }
        public EmergencyAccess EmergencyOverride { get; set; }
        public TimeZoneInfo OperatingTimeZone { get; set; }
        public bool EnforceBusinessHours { get; set; }
    }

    public class EmergencyAccess
    {
        public bool Allowed { get; set; }
        public int RequiredApprovers { get; set; }
        public string[] EmergencyApproverRoles { get; set; }
        public TimeSpan MaxEmergencyDuration { get; set; }
        public bool RequirePostMortem { get; set; }
    }

    public class UsageQuotas
    {
        public RateLimits OperationRateLimits { get; set; }
        public Dictionary<string, long> DataVolumeLimits { get; set; }
        public Dictionary<string, int> ConcurrentOperationLimits { get; set; }
        public QuotaEnforcement EnforcementPolicy { get; set; }
    }

    public class RateLimits
    {
        public int OperationsPerSecond { get; set; }
        public int OperationsPerMinute { get; set; }
        public int OperationsPerHour { get; set; }
        public Dictionary<string, int> OperationSpecificLimits { get; set; }
    }

    public class QuotaEnforcement
    {
        public bool HardEnforcement { get; set; }
        public int WarningThresholdPercent { get; set; }
        public string[] NotificationTargets { get; set; }
        public bool AutomaticQuotaIncrease { get; set; }
    }

    public class KeyPurposePolicy
    {
        public string[] AllowedOperations { get; set; }
        public string[] AllowedDataTypes { get; set; }
        public Dictionary<string, long> DataSizeLimits { get; set; }
        public string[] AllowedAlgorithms { get; set; }
        public KeyUsageRestrictions UsageRestrictions { get; set; }
    }

    public class KeyUsageRestrictions
    {
        public bool AllowKeyWrapping { get; set; }
        public bool AllowKeyDerivation { get; set; }
        public bool AllowExport { get; set; } = false; // Default to non-exportable
        public bool RequireDataEncryption { get; set; }
        public string[] AllowedKeyUsages { get; set; }
    }

    public class ApprovalPolicy
    {
        public string[] RequiredApproverRoles { get; set; }
        public int MinimumApprovers { get; set; }
        public TimeSpan ApprovalExpiration { get; set; }
        public AuthenticationRequirements AuthRequirements { get; set; }
        public string[] NotificationTargets { get; set; }
        public bool RequireJustification { get; set; }
    }

    public class AuthenticationRequirements
    {
        public bool RequireMfa { get; set; }
        public string[] AllowedMfaTypes { get; set; }
        public bool RequireHardwareToken { get; set; }
        public int MinimumAuthStrength { get; set; }
    }

    public class CompliancePolicy
    {
        public string[] RequiredClassifications { get; set; }
        public string[] RequiredCertifications { get; set; }
        public AuditRequirements AuditControls { get; set; }
        public DataResidencyPolicy ResidencyControls { get; set; }
        public RetentionPolicy RetentionControls { get; set; }
    }

    public class AuditRequirements
    {
        public bool RequireAuditLog { get; set; }
        public string[] RequiredAuditEvents { get; set; }
        public int AuditRetentionDays { get; set; }
        public bool RequireSignedAuditLogs { get; set; }
    }

    public class DataResidencyPolicy
    {
        public string[] AllowedRegions { get; set; }
        public bool EnforceDataSovereignty { get; set; }
        public string[] ProhibitedRegions { get; set; }
        public Dictionary<string, string[]> DataTypeResidencyRules { get; set; }
    }

    public class RetentionPolicy
    {
        public int MinimumRetentionDays { get; set; }
        public int MaximumRetentionDays { get; set; }
        public bool RequireSecureDelete { get; set; }
        public string[] RetentionTriggers { get; set; }
    }

    public class ApplicationPolicy
    {
        public Dictionary<string, string[]> AllowedOperationsByApp { get; set; }
        public Dictionary<string, string[]> AllowedOperationsByEnv { get; set; }
        public string[] AllowedKeyPrefixes { get; set; }
        public Dictionary<string, int> AppSpecificQuotas { get; set; }
        public ApplicationRestrictions Restrictions { get; set; }
    }

    public class ApplicationRestrictions
    {
        public bool RequireAppAuthentication { get; set; }
        public string[] AllowedAuthMethods { get; set; }
        public bool RequireServicePrincipal { get; set; }
        public Dictionary<string, string[]> RequiredAppRoles { get; set; }
    }

    public class HardwareSecurityPolicy
    {
        public bool RequireHsmBacked { get; set; }
        public string[] AllowedHsmProviders { get; set; }
        public string MinimumFipsLevel { get; set; }
        public HsmKeyProperties KeyProperties { get; set; }
        public BackupPolicy BackupControls { get; set; }
    }

    public class HsmKeyProperties
    {
        public bool NonExportable { get; set; } = true; // Default to non-exportable
        public bool RequireKeyWrapping { get; set; }
        public string[] AllowedKeyOrigins { get; set; }
        public string MinimumKeySize { get; set; }
    }

    public class BackupPolicy
    {
        public bool AllowBackup { get; set; }
        public string[] AllowedBackupLocations { get; set; }
        public bool RequireBackupEncryption { get; set; }
        public int BackupRetentionDays { get; set; }
    }

    public class KeyExportPolicy
    {
        public bool AllowExport { get; set; } = false; // Default to non-exportable
        public ExportRestrictions Restrictions { get; set; }
        public ExportAuditPolicy AuditPolicy { get; set; }
        public EmergencyExportPolicy EmergencyPolicy { get; set; }
    }

    public class ExportRestrictions
    {
        public bool AllowPlaintextExport { get; set; } = false;
        public bool RequireKeyWrapping { get; set; } = true;
        public string[] AllowedExportFormats { get; set; }
        public string[] AllowedRecipientKeys { get; set; }
    }

    public class ExportAuditPolicy
    {
        public bool RequireAuditLog { get; set; } = true;
        public bool NotifyOnExport { get; set; } = true;
        public string[] NotificationTargets { get; set; }
        public bool RequireExportJustification { get; set; } = true;
    }

    public class EmergencyExportPolicy
    {
        public bool AllowEmergencyExport { get; set; } = false;
        public int RequiredApprovers { get; set; } = 3;
        public string[] EmergencyApproverRoles { get; set; }
        public bool RequirePostMortemReport { get; set; } = true;
    }
}
