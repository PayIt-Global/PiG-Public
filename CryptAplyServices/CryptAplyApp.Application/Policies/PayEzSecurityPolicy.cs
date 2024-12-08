using System;
using System.Collections.Generic;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Policies
{
    public static class PayEzSecurityPolicy
    {
        public static SecurityPolicy GetDefaultPolicy()
        {
            return new SecurityPolicy
            {
                ContextControls = new ContextBasedAccess
                {
                    RequiredLabels = new Dictionary<string, string[]>
                    {
                        { "environment", new[] { "production", "staging" } },
                        { "service", new[] { "payez-core", "payez-api" } }
                    },
                    NetworkControls = new NetworkPolicy
                    {
                        RequirePrivateEndpoint = true,
                        AllowInternetAccess = false
                    }
                },

                TimeControls = new TimeWindowControls
                {
                    EnforceBusinessHours = true,
                    EmergencyOverride = new EmergencyAccess
                    {
                        Allowed = true,
                        RequiredApprovers = 2,
                        EmergencyApproverRoles = new[] { "SecurityOfficer", "ComplianceOfficer" }
                    }
                },

                QuotaControls = new UsageQuotas
                {
                    OperationRateLimits = new RateLimits
                    {
                        OperationsPerSecond = 1000,
                        OperationSpecificLimits = new Dictionary<string, int>
                        {
                            { "encrypt", 5000 },
                            { "decrypt", 5000 }
                        }
                    }
                },

                PurposeControls = new KeyPurposePolicy
                {
                    AllowedOperations = new[] 
                    { 
                        "encrypt", 
                        "decrypt", 
                        "sign", 
                        "verify" 
                    },
                    UsageRestrictions = new KeyUsageRestrictions
                    {
                        AllowExport = false, // Enforce non-exportable
                        AllowKeyWrapping = false,
                        AllowKeyDerivation = false,
                        RequireDataEncryption = true
                    }
                },

                ApprovalControls = new ApprovalPolicy
                {
                    RequiredApproverRoles = new[] { "SecurityOfficer", "KeyCustodian" },
                    MinimumApprovers = 2,
                    AuthRequirements = new AuthenticationRequirements
                    {
                        RequireMfa = true,
                        RequireHardwareToken = true
                    }
                },

                ComplianceControls = new CompliancePolicy
                {
                    RequiredCertifications = new[] { "PCI-DSS", "SOC2" },
                    AuditControls = new AuditRequirements
                    {
                        RequireAuditLog = true,
                        RequiredAuditEvents = new[] 
                        { 
                            "key.create", 
                            "key.rotate", 
                            "key.use",
                            "key.delete" 
                        },
                        RequireSignedAuditLogs = true
                    },
                    ResidencyControls = new DataResidencyPolicy
                    {
                        AllowedRegions = new[] { "us-west", "us-east" },
                        EnforceDataSovereignty = true
                    }
                },

                ApplicationControls = new ApplicationPolicy
                {
                    AllowedOperationsByApp = new Dictionary<string, string[]>
                    {
                        { 
                            "PayEz", 
                            new[] 
                            { 
                                "encrypt", 
                                "decrypt", 
                                "sign", 
                                "verify" 
                            } 
                        }
                    },
                    Restrictions = new ApplicationRestrictions
                    {
                        RequireServicePrincipal = true,
                        RequiredAppRoles = new Dictionary<string, string[]>
                        {
                            { "PayEz", new[] { "KeyUser", "TokenizationService" } }
                        }
                    }
                },

                HsmControls = new HardwareSecurityPolicy
                {
                    RequireHsmBacked = true,
                    MinimumFipsLevel = "FIPS 140-2 Level 3",
                    KeyProperties = new HsmKeyProperties
                    {
                        NonExportable = true,
                        RequireKeyWrapping = true,
                        AllowedKeyOrigins = new[] { "HSM" },
                        MinimumKeySize = "2048"
                    },
                    BackupControls = new BackupPolicy
                    {
                        AllowBackup = true,
                        RequireBackupEncryption = true,
                        BackupRetentionDays = 365
                    }
                },

                ExportControls = new KeyExportPolicy
                {
                    AllowExport = false,
                    Restrictions = new ExportRestrictions
                    {
                        AllowPlaintextExport = false,
                        RequireKeyWrapping = true,
                        AllowedExportFormats = Array.Empty<string>() // No exports allowed
                    },
                    AuditPolicy = new ExportAuditPolicy
                    {
                        RequireAuditLog = true,
                        NotifyOnExport = true,
                        NotificationTargets = new[] 
                        { 
                            "security@company.com",
                            "compliance@company.com" 
                        },
                        RequireExportJustification = true
                    },
                    EmergencyPolicy = new EmergencyExportPolicy
                    {
                        AllowEmergencyExport = false, // Even emergency exports not allowed
                        RequiredApprovers = 3,
                        RequirePostMortemReport = true
                    }
                }
            };
        }
    }
}
