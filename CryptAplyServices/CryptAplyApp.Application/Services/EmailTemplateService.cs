using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Services
{
    public class EmailTemplateService
    {
        private readonly ILogger<EmailTemplateService> _logger;
        private readonly Dictionary<NotificationType, EmailTemplate> _templates;

        public EmailTemplateService(ILogger<EmailTemplateService> logger)
        {
            _logger = logger;
            _templates = InitializeTemplates();
        }

        private Dictionary<NotificationType, EmailTemplate> InitializeTemplates()
        {
            var templates = new Dictionary<NotificationType, EmailTemplate>
            {
                {
                    NotificationType.KeyExpiringSoon,
                    new EmailTemplate
                    {
                        Name = "KeyExpiringSoon",
                        Subject = "Key Expiration Alert - Action Required: {KeyName}",
                        HtmlBody = @"
                            <h2>Key Expiration Alert</h2>
                            <p>The following cryptographic key is approaching its expiration date:</p>
                            <ul>
                                <li><strong>Key Name:</strong> {KeyName}</li>
                                <li><strong>Key ID:</strong> {KeyId}</li>
                                <li><strong>Expiration Date:</strong> {ExpirationDate}</li>
                                <li><strong>Days Remaining:</strong> {DaysRemaining}</li>
                                <li><strong>Environment:</strong> {Environment}</li>
                            </ul>
                            <h3>Impact Assessment</h3>
                            <p>The following applications will be affected:</p>
                            <ul>{ApplicationList}</ul>
                            <div class='action-required'>
                                <h3>Action Required</h3>
                                <p>Please take one of the following actions before the key expires:</p>
                                <ol>
                                    <li><a href='{RotateLink}'>Rotate the key</a></li>
                                    <li><a href='{ExtendLink}'>Extend the expiration date</a></li>
                                    <li><a href='{ReplaceLink}'>Replace with a new key</a></li>
                                </ol>
                            </div>
                            <p class='note'>Note: Failure to take action will result in service disruption.</p>",
                        NotificationType = NotificationType.KeyExpiringSoon,
                        DefaultPriority = NotificationPriority.High,
                        RequiresAcknowledgment = true
                    }
                },
                {
                    NotificationType.ExcessiveUsageAlert,
                    new EmailTemplate
                    {
                        Name = "ExcessiveUsageAlert",
                        Subject = "Unusual Key Usage Detected - {KeyName}",
                        HtmlBody = @"
                            <h2>Unusual Key Usage Pattern Detected</h2>
                            <div class='alert alert-warning'>
                                <p>We've detected unusual usage patterns for the following key:</p>
                            </div>
                            <ul>
                                <li><strong>Key Name:</strong> {KeyName}</li>
                                <li><strong>Time Period:</strong> {TimePeriod}</li>
                                <li><strong>Normal Usage Rate:</strong> {NormalRate} operations/hour</li>
                                <li><strong>Current Usage Rate:</strong> {CurrentRate} operations/hour</li>
                                <li><strong>Increase:</strong> {IncreasePercentage}%</li>
                            </ul>
                            <h3>Usage Breakdown</h3>
                            <ul>
                                <li><strong>Top Applications:</strong> {TopApplications}</li>
                                <li><strong>Top Operations:</strong> {TopOperations}</li>
                                <li><strong>Geographic Distribution:</strong> {GeoDistribution}</li>
                            </ul>
                            <div class='action-section'>
                                <a href='{InvestigateLink}' class='button'>Investigate Activity</a>
                                <a href='{AdjustThresholdsLink}' class='button'>Adjust Thresholds</a>
                            </div>",
                        NotificationType = NotificationType.ExcessiveUsageAlert,
                        DefaultPriority = NotificationPriority.Medium,
                        RequiresAcknowledgment = true
                    }
                },
                {
                    NotificationType.CompromiseAlert,
                    new EmailTemplate
                    {
                        Name = "CompromiseAlert",
                        Subject = "CRITICAL: Potential Key Compromise Detected - {KeyName}",
                        HtmlBody = @"
                            <div class='alert alert-critical'>
                                <h2>⚠️ CRITICAL: Potential Key Compromise Detected</h2>
                            </div>
                            <p>Our security monitoring has detected potential compromise indicators for:</p>
                            <ul>
                                <li><strong>Key Name:</strong> {KeyName}</li>
                                <li><strong>Detection Time:</strong> {DetectionTime}</li>
                                <li><strong>Severity:</strong> {Severity}</li>
                            </ul>
                            <h3>Compromise Indicators</h3>
                            <ul>{CompromiseIndicators}</ul>
                            <div class='emergency-actions'>
                                <h3>Required Immediate Actions</h3>
                                <ol>
                                    <li><a href='{SuspendLink}' class='button-critical'>Suspend Key</a></li>
                                    <li><a href='{RevokeLink}' class='button-critical'>Revoke Access</a></li>
                                    <li><a href='{InvestigateLink}' class='button'>Start Investigation</a></li>
                                </ol>
                            </div>
                            <p class='note'>This alert has been automatically escalated to the security team.</p>",
                        NotificationType = NotificationType.CompromiseAlert,
                        DefaultPriority = NotificationPriority.Critical,
                        RequiresMFA = true,
                        RequiresAcknowledgment = true,
                        ExpiresAfter = TimeSpan.FromHours(4)
                    }
                },
                {
                    NotificationType.ComplianceReport,
                    new EmailTemplate
                    {
                        Name = "ComplianceReport",
                        Subject = "Monthly Key Compliance Report - {Month} {Year}",
                        HtmlBody = @"
                            <h2>Monthly Key Compliance Report</h2>
                            <p>Period: {Month} {Year}</p>
                            
                            <h3>Compliance Summary</h3>
                            <ul>
                                <li>Total Active Keys: {TotalKeys}</li>
                                <li>Compliant Keys: {CompliantKeys}</li>
                                <li>Non-Compliant Keys: {NonCompliantKeys}</li>
                                <li>Overall Compliance Rate: {ComplianceRate}%</li>
                            </ul>

                            <h3>Key Rotation Status</h3>
                            <ul>
                                <li>Keys Rotated This Month: {RotatedKeys}</li>
                                <li>Overdue Rotations: {OverdueRotations}</li>
                                <li>Upcoming Rotations: {UpcomingRotations}</li>
                            </ul>

                            <h3>Security Metrics</h3>
                            <ul>
                                <li>Failed Access Attempts: {FailedAccess}</li>
                                <li>Unusual Usage Patterns: {UnusualPatterns}</li>
                                <li>Policy Violations: {PolicyViolations}</li>
                            </ul>

                            <div class='action-section'>
                                <a href='{DetailedReportLink}' class='button'>View Detailed Report</a>
                                <a href='{ComplianceDashboardLink}' class='button'>Open Compliance Dashboard</a>
                            </div>",
                        NotificationType = NotificationType.ComplianceReport,
                        DefaultPriority = NotificationPriority.Medium,
                        RequiresAcknowledgment = false
                    }
                },
                {
                    NotificationType.UnusedKeyAlert,
                    new EmailTemplate
                    {
                        Name = "UnusedKeyAlert",
                        Subject = "Unused Cryptographic Key Alert - {KeyName}",
                        HtmlBody = @"
                            <div class='alert alert-warning'>
                                <h2>Unused Key Detection</h2>
                                <p>A cryptographic key has been detected with no usage in the past {InactiveDays} days:</p>
                            </div>
                            <div class='key-details'>
                                <h3>Key Details</h3>
                                <ul>
                                    <li><strong>Key Name:</strong> {KeyName}</li>
                                    <li><strong>Key ID:</strong> {KeyId}</li>
                                    <li><strong>Environment:</strong> {Environment}</li>
                                    <li><strong>Last Used:</strong> {LastUsedDate}</li>
                                    <li><strong>Created Date:</strong> {CreatedDate}</li>
                                </ul>
                            </div>
                            <div class='recommendations'>
                                <h3>Recommended Actions</h3>
                                <ol>
                                    <li><a href='{ReviewLink}'>Review key usage patterns</a></li>
                                    <li><a href='{ArchiveLink}'>Archive the key</a></li>
                                    <li><a href='{DeleteLink}'>Delete the key</a></li>
                                </ol>
                            </div>
                            <div class='impact-analysis'>
                                <h3>Impact Analysis</h3>
                                <p>The following applications are configured to use this key but haven't made any requests:</p>
                                <ul>{ApplicationList}</ul>
                            </div>",
                        NotificationType = NotificationType.UnusedKeyAlert,
                        DefaultPriority = NotificationPriority.Medium,
                        RequiresAcknowledgment = true
                    }
                },
                {
                    NotificationType.PolicyViolation,
                    new EmailTemplate
                    {
                        Name = "PolicyViolation",
                        Subject = "Key Policy Violation Detected - {KeyName}",
                        HtmlBody = @"
                            <div class='alert alert-critical'>
                                <h2>Policy Violation Alert</h2>
                                <p>A key policy violation has been detected:</p>
                            </div>
                            <div class='violation-details'>
                                <h3>Violation Details</h3>
                                <ul>
                                    <li><strong>Key Name:</strong> {KeyName}</li>
                                    <li><strong>Policy Name:</strong> {PolicyName}</li>
                                    <li><strong>Violation Type:</strong> {ViolationType}</li>
                                    <li><strong>Detection Time:</strong> {DetectionTime}</li>
                                    <li><strong>Severity:</strong> {Severity}</li>
                                </ul>
                            </div>
                            <div class='violation-description'>
                                <h3>Description</h3>
                                <p>{ViolationDescription}</p>
                                <pre>{ViolationDetails}</pre>
                            </div>
                            <div class='required-actions'>
                                <h3>Required Actions</h3>
                                <ol>
                                    <li><a href='{ReviewLink}'>Review violation details</a></li>
                                    <li><a href='{RemediateLink}'>Remediate violation</a></li>
                                    <li><a href='{ExemptLink}'>Request policy exemption</a></li>
                                </ol>
                            </div>",
                        NotificationType = NotificationType.PolicyViolation,
                        DefaultPriority = NotificationPriority.High,
                        RequiresAcknowledgment = true
                    }
                },
                {
                    NotificationType.MaintenanceAlert,
                    new EmailTemplate
                    {
                        Name = "MaintenanceAlert",
                        Subject = "Key Infrastructure Maintenance - {MaintenanceType}",
                        HtmlBody = @"
                            <div class='alert alert-info'>
                                <h2>Scheduled Maintenance Notice</h2>
                                <p>The following key infrastructure maintenance is scheduled:</p>
                            </div>
                            <div class='maintenance-details'>
                                <h3>Maintenance Details</h3>
                                <ul>
                                    <li><strong>Type:</strong> {MaintenanceType}</li>
                                    <li><strong>Start Time:</strong> {StartTime}</li>
                                    <li><strong>Duration:</strong> {Duration}</li>
                                    <li><strong>Impact Level:</strong> {ImpactLevel}</li>
                                </ul>
                            </div>
                            <div class='impact-assessment'>
                                <h3>Service Impact</h3>
                                <p>{ImpactDescription}</p>
                                <h4>Affected Services:</h4>
                                <ul>{AffectedServices}</ul>
                            </div>
                            <div class='preparation-steps'>
                                <h3>Required Preparation</h3>
                                <ol>{PreparationSteps}</ol>
                            </div>
                            <div class='contingency'>
                                <h3>Contingency Plan</h3>
                                <p>{ContingencyPlan}</p>
                                <a href='{ContingencyLink}' class='button'>View Detailed Plan</a>
                            </div>",
                        NotificationType = NotificationType.MaintenanceAlert,
                        DefaultPriority = NotificationPriority.Medium,
                        RequiresAcknowledgment = true
                    }
                },
                {
                    NotificationType.BackupReminder,
                    new EmailTemplate
                    {
                        Name = "BackupReminder",
                        Subject = "Key Backup Required - {KeyName}",
                        HtmlBody = @"
                            <div class='alert alert-warning'>
                                <h2>Key Backup Required</h2>
                                <p>The following key requires a backup:</p>
                            </div>
                            <div class='key-details'>
                                <h3>Key Information</h3>
                                <ul>
                                    <li><strong>Key Name:</strong> {KeyName}</li>
                                    <li><strong>Last Backup:</strong> {LastBackupDate}</li>
                                    <li><strong>Backup Status:</strong> {BackupStatus}</li>
                                    <li><strong>Risk Level:</strong> {RiskLevel}</li>
                                </ul>
                            </div>
                            <div class='backup-requirements'>
                                <h3>Backup Requirements</h3>
                                <ul>
                                    <li><strong>Required Frequency:</strong> {RequiredFrequency}</li>
                                    <li><strong>Retention Period:</strong> {RetentionPeriod}</li>
                                    <li><strong>Storage Location:</strong> {StorageLocation}</li>
                                </ul>
                            </div>
                            <div class='backup-procedure'>
                                <h3>Backup Procedure</h3>
                                <ol>{BackupSteps}</ol>
                                <a href='{BackupLink}' class='button'>Start Backup</a>
                            </div>",
                        NotificationType = NotificationType.BackupReminder,
                        DefaultPriority = NotificationPriority.High,
                        RequiresAcknowledgment = true
                    }
                },
                {
                    NotificationType.EmergencyAccess,
                    new EmailTemplate
                    {
                        Name = "EmergencyAccess",
                        Subject = "URGENT: Emergency Key Access Request",
                        HtmlBody = @"
                            <div class='alert alert-critical'>
                                <h2>Emergency Access Request</h2>
                                <p>An emergency access request has been initiated:</p>
                            </div>
                            <div class='request-details'>
                                <h3>Request Details</h3>
                                <ul>
                                    <li><strong>Requestor:</strong> {RequestorName}</li>
                                    <li><strong>Role:</strong> {RequestorRole}</li>
                                    <li><strong>Key Name:</strong> {KeyName}</li>
                                    <li><strong>Request Time:</strong> {RequestTime}</li>
                                    <li><strong>Justification:</strong> {Justification}</li>
                                </ul>
                            </div>
                            <div class='verification'>
                                <h3>Identity Verification</h3>
                                <ul>
                                    <li><strong>MFA Status:</strong> {MFAStatus}</li>
                                    <li><strong>Location:</strong> {AccessLocation}</li>
                                    <li><strong>Device:</strong> {DeviceInfo}</li>
                                </ul>
                            </div>
                            <div class='actions required-immediate'>
                                <h3>Required Actions</h3>
                                <p>Please review and take action within {TimeLimit} minutes:</p>
                                <div class='action-buttons'>
                                    <a href='{ApproveLink}' class='button button-approve'>Approve Access</a>
                                    <a href='{DenyLink}' class='button button-deny'>Deny Access</a>
                                    <a href='{EscalateLink}' class='button button-escalate'>Escalate</a>
                                </div>
                            </div>
                            <div class='audit-note'>
                                <p>Note: All actions will be logged for audit purposes.</p>
                            </div>",
                        NotificationType = NotificationType.EmergencyAccess,
                        DefaultPriority = NotificationPriority.Critical,
                        RequiresMFA = true,
                        RequiresAcknowledgment = true,
                        ExpiresAfter = TimeSpan.FromMinutes(30)
                    }
                },
                {
                    NotificationType.PciKeyRotationRequired,
                    new EmailTemplate
                    {
                        Name = "PciKeyRotationRequired",
                        Subject = "PCI DSS Required: Cryptographic Key Rotation Due - {KeyName}",
                        HtmlBody = @"
                            <div class='alert alert-warning pci-alert'>
                                <h2>PCI DSS Key Rotation Requirement</h2>
                                <p>Per PCI DSS requirement 3.6.4, the following cryptographic key requires rotation:</p>
                            </div>
                            <div class='key-details'>
                                <h3>Key Information</h3>
                                <ul>
                                    <li><strong>Key Name:</strong> {KeyName}</li>
                                    <li><strong>Key ID:</strong> {KeyId}</li>
                                    <li><strong>Key Type:</strong> {KeyType}</li>
                                    <li><strong>Current Age:</strong> {KeyAge} days</li>
                                    <li><strong>PCI DSS Maximum Age:</strong> {MaxKeyAge} days</li>
                                    <li><strong>Days Until Required Rotation:</strong> {DaysUntilRotation}</li>
                                    <li><strong>Cardholder Data Elements:</strong> {CardholderDataElements}</li>
                                </ul>
                            </div>
                            <div class='compliance-impact'>
                                <h3>Compliance Impact</h3>
                                <ul>
                                    <li><strong>PCI DSS Requirement:</strong> 3.6.4 - Cryptographic key changes for keys that have reached the end of their cryptoperiod</li>
                                    <li><strong>Risk Level:</strong> {RiskLevel}</li>
                                    <li><strong>Compliance Status:</strong> {ComplianceStatus}</li>
                                </ul>
                            </div>
                            <div class='affected-systems'>
                                <h3>Affected Payment Systems</h3>
                                <ul>{AffectedSystems}</ul>
                            </div>
                            <div class='rotation-procedure'>
                                <h3>Required Actions</h3>
                                <ol>
                                    <li>Review key usage and dependencies</li>
                                    <li>Schedule rotation during approved change window</li>
                                    <li>Ensure all key custodians are available</li>
                                    <li>Verify backup procedures are current</li>
                                    <li>Document rotation in key management logs</li>
                                </ol>
                                <div class='action-buttons'>
                                    <a href='{ScheduleRotationLink}' class='button'>Schedule Rotation</a>
                                    <a href='{ReviewProcedureLink}' class='button'>Review Procedure</a>
                                </div>
                            </div>",
                        NotificationType = NotificationType.PciKeyRotationRequired,
                        DefaultPriority = NotificationPriority.High,
                        RequiresAcknowledgment = true
                    }
                },
                {
                    NotificationType.PciKeyAccessAudit,
                    new EmailTemplate
                    {
                        Name = "PciKeyAccessAudit",
                        Subject = "PCI DSS Key Access Audit Report - {Period}",
                        HtmlBody = @"
                            <div class='alert alert-info pci-alert'>
                                <h2>PCI DSS Key Access Audit Report</h2>
                                <p>Per PCI DSS requirement 3.6.8, the following key access audit report has been generated:</p>
                            </div>
                            <div class='audit-period'>
                                <h3>Audit Period</h3>
                                <ul>
                                    <li><strong>Start Date:</strong> {StartDate}</li>
                                    <li><strong>End Date:</strong> {EndDate}</li>
                                    <li><strong>Report Generation:</strong> {GenerationTime}</li>
                                </ul>
                            </div>
                            <div class='access-summary'>
                                <h3>Access Summary</h3>
                                <ul>
                                    <li><strong>Total Access Attempts:</strong> {TotalAccessAttempts}</li>
                                    <li><strong>Successful Access:</strong> {SuccessfulAccess}</li>
                                    <li><strong>Failed Access:</strong> {FailedAccess}</li>
                                    <li><strong>Unauthorized Attempts:</strong> {UnauthorizedAttempts}</li>
                                </ul>
                            </div>
                            <div class='key-custodian-activity'>
                                <h3>Key Custodian Activity</h3>
                                <table class='audit-table'>
                                    <tr>
                                        <th>Custodian</th>
                                        <th>Role</th>
                                        <th>Access Count</th>
                                        <th>Last Access</th>
                                    </tr>
                                    {CustodianActivityRows}
                                </table>
                            </div>
                            <div class='suspicious-activity'>
                                <h3>Suspicious Activity</h3>
                                <ul>{SuspiciousActivityList}</ul>
                            </div>
                            <div class='compliance-actions'>
                                <h3>Required Actions</h3>
                                <ol>
                                    <li>Review all access attempts</li>
                                    <li>Investigate any suspicious activity</li>
                                    <li>Update access controls if needed</li>
                                    <li>Document review in compliance logs</li>
                                </ol>
                                <div class='action-buttons'>
                                    <a href='{DetailedReportLink}' class='button'>View Detailed Report</a>
                                    <a href='{AcknowledgeLink}' class='button'>Acknowledge Review</a>
                                </div>
                            </div>",
                        NotificationType = NotificationType.PciKeyAccessAudit,
                        DefaultPriority = NotificationPriority.Medium,
                        RequiresAcknowledgment = true
                    }
                },
                {
                    NotificationType.PciComplianceViolation,
                    new EmailTemplate
                    {
                        Name = "PciComplianceViolation",
                        Subject = "URGENT: PCI DSS Compliance Violation - {ViolationType}",
                        HtmlBody = @"
                            <div class='alert alert-critical pci-alert'>
                                <h2>PCI DSS Compliance Violation Alert</h2>
                                <p>A PCI DSS compliance violation has been detected in the key management system:</p>
                            </div>
                            <div class='violation-details'>
                                <h3>Violation Details</h3>
                                <ul>
                                    <li><strong>Violation Type:</strong> {ViolationType}</li>
                                    <li><strong>PCI Requirement:</strong> {PciRequirement}</li>
                                    <li><strong>Detection Time:</strong> {DetectionTime}</li>
                                    <li><strong>Severity:</strong> {Severity}</li>
                                    <li><strong>Affected Keys:</strong> {AffectedKeys}</li>
                                </ul>
                            </div>
                            <div class='violation-description'>
                                <h3>Description</h3>
                                <p>{ViolationDescription}</p>
                                <pre class='violation-details'>{TechnicalDetails}</pre>
                            </div>
                            <div class='cardholder-data-impact'>
                                <h3>Cardholder Data Impact Assessment</h3>
                                <ul>
                                    <li><strong>Data at Risk:</strong> {DataAtRisk}</li>
                                    <li><strong>Exposure Period:</strong> {ExposurePeriod}</li>
                                    <li><strong>Affected Systems:</strong> {AffectedSystems}</li>
                                </ul>
                            </div>
                            <div class='immediate-actions'>
                                <h3>Required Immediate Actions</h3>
                                <ol>
                                    <li>Review violation details</li>
                                    <li>Implement corrective measures</li>
                                    <li>Document incident response</li>
                                    <li>Update security controls</li>
                                    <li>Notify QSA if required</li>
                                </ol>
                                <div class='action-buttons'>
                                    <a href='{RemediationLink}' class='button button-critical'>Start Remediation</a>
                                    <a href='{DocumentationLink}' class='button'>Document Response</a>
                                    <a href='{QsaNotificationLink}' class='button'>Notify QSA</a>
                                </div>
                            </div>
                            <div class='compliance-note'>
                                <p>Note: This violation must be addressed within {RemediationTimeframe} hours to maintain PCI DSS compliance.</p>
                            </div>",
                        NotificationType = NotificationType.PciComplianceViolation,
                        DefaultPriority = NotificationPriority.Critical,
                        RequiresMFA = true,
                        RequiresAcknowledgment = true,
                        ExpiresAfter = TimeSpan.FromHours(4)
                    }
                },
                {
                    NotificationType.PciKeyDualControlEvent,
                    new EmailTemplate
                    {
                        Name = "PciKeyDualControlEvent",
                        Subject = "PCI DSS: Dual Control Authorization Required - {OperationType}",
                        HtmlBody = @"
                            <div class='alert alert-warning pci-alert'>
                                <h2>Dual Control Authorization Required</h2>
                                <p>Per PCI DSS requirement 3.6.6, the following key operation requires dual control authorization:</p>
                            </div>
                            <div class='operation-details'>
                                <h3>Operation Details</h3>
                                <ul>
                                    <li><strong>Operation Type:</strong> {OperationType}</li>
                                    <li><strong>Requested By:</strong> {RequestorName}</li>
                                    <li><strong>Request Time:</strong> {RequestTime}</li>
                                    <li><strong>Key Material:</strong> {KeyIdentifier}</li>
                                    <li><strong>Business Justification:</strong> {Justification}</li>
                                </ul>
                            </div>
                            <div class='authorization-status'>
                                <h3>Authorization Status</h3>
                                <ul>
                                    <li><strong>Primary Custodian:</strong> {PrimaryCustodian} - {PrimaryStatus}</li>
                                    <li><strong>Secondary Custodian:</strong> {SecondaryCustodian} - {SecondaryStatus}</li>
                                    <li><strong>Deadline:</strong> {AuthorizationDeadline}</li>
                                </ul>
                            </div>
                            <div class='action-buttons'>
                                <a href='{AuthorizeLink}' class='button'>Authorize Operation</a>
                                <a href='{RejectLink}' class='button button-danger'>Reject Request</a>
                                <a href='{AuditLogLink}' class='button'>View Audit Log</a>
                            </div>",
                        NotificationType = NotificationType.PciKeyDualControlEvent,
                        DefaultPriority = NotificationPriority.High,
                        RequiresMFA = true,
                        RequiresAcknowledgment = true,
                        ExpiresAfter = TimeSpan.FromHours(2)
                    }
                },
                {
                    NotificationType.PciKeyInventoryCheck,
                    new EmailTemplate
                    {
                        Name = "PciKeyInventoryCheck",
                        Subject = "PCI DSS: Cryptographic Key Inventory Review Required",
                        HtmlBody = @"
                            <div class='alert alert-info pci-alert'>
                                <h2>Key Inventory Review Required</h2>
                                <p>Per PCI DSS requirement 3.6.3, a periodic review of the cryptographic key inventory is required:</p>
                            </div>
                            <div class='inventory-summary'>
                                <h3>Current Inventory Status</h3>
                                <ul>
                                    <li><strong>Total Active Keys:</strong> {ActiveKeyCount}</li>
                                    <li><strong>Keys Pending Rotation:</strong> {PendingRotationCount}</li>
                                    <li><strong>Retired Keys:</strong> {RetiredKeyCount}</li>
                                    <li><strong>Emergency Keys:</strong> {EmergencyKeyCount}</li>
                                </ul>
                            </div>
                            <div class='key-distribution'>
                                <h3>Key Distribution by Type</h3>
                                <ul>
                                    <li><strong>Data Encryption Keys:</strong> {DataEncryptionKeyCount}</li>
                                    <li><strong>Key Encryption Keys:</strong> {KeyEncryptionKeyCount}</li>
                                    <li><strong>MAC Keys:</strong> {MacKeyCount}</li>
                                    <li><strong>PIN Keys:</strong> {PinKeyCount}</li>
                                </ul>
                            </div>
                            <div class='review-checklist'>
                                <h3>Required Review Items</h3>
                                <ol>
                                    <li>Verify all keys are accounted for</li>
                                    <li>Confirm key custodian assignments</li>
                                    <li>Check key expiration dates</li>
                                    <li>Review key usage patterns</li>
                                    <li>Validate backup key availability</li>
                                    <li>Document any discrepancies</li>
                                </ol>
                            </div>
                            <div class='action-buttons'>
                                <a href='{StartReviewLink}' class='button'>Start Review</a>
                                <a href='{InventoryReportLink}' class='button'>Download Full Inventory</a>
                            </div>",
                        NotificationType = NotificationType.PciKeyInventoryCheck,
                        DefaultPriority = NotificationPriority.Medium,
                        RequiresAcknowledgment = true
                    }
                },
                {
                    NotificationType.PciKeyBackupVerification,
                    new EmailTemplate
                    {
                        Name = "PciKeyBackupVerification",
                        Subject = "PCI DSS: Key Backup Verification Required",
                        HtmlBody = @"
                            <div class='alert alert-warning pci-alert'>
                                <h2>Key Backup Verification Required</h2>
                                <p>Per PCI DSS requirement 3.6.7, verification of cryptographic key backups is required:</p>
                            </div>
                            <div class='backup-status'>
                                <h3>Backup Status</h3>
                                <ul>
                                    <li><strong>Last Backup Date:</strong> {LastBackupDate}</li>
                                    <li><strong>Backup Location:</strong> {BackupLocation}</li>
                                    <li><strong>Keys in Backup:</strong> {BackupKeyCount}</li>
                                    <li><strong>Last Verification:</strong> {LastVerificationDate}</li>
                                </ul>
                            </div>
                            <div class='verification-steps'>
                                <h3>Required Verification Steps</h3>
                                <ol>
                                    <li>Retrieve secure backup media</li>
                                    <li>Verify backup integrity</li>
                                    <li>Test key restoration process</li>
                                    <li>Validate key material</li>
                                    <li>Document verification results</li>
                                    <li>Update backup verification logs</li>
                                </ol>
                            </div>
                            <div class='key-custodians'>
                                <h3>Required Key Custodians</h3>
                                <ul>
                                    <li><strong>Primary Custodian:</strong> {PrimaryCustodian}</li>
                                    <li><strong>Backup Custodian:</strong> {BackupCustodian}</li>
                                    <li><strong>Security Officer:</strong> {SecurityOfficer}</li>
                                </ul>
                            </div>
                            <div class='action-buttons'>
                                <a href='{ScheduleVerificationLink}' class='button'>Schedule Verification</a>
                                <a href='{BackupProcedureLink}' class='button'>View Procedure</a>
                            </div>",
                        NotificationType = NotificationType.PciKeyBackupVerification,
                        DefaultPriority = NotificationPriority.High,
                        RequiresMFA = true,
                        RequiresAcknowledgment = true
                    }
                },
                {
                    NotificationType.PciKeyCompromiseAlert,
                    new EmailTemplate
                    {
                        Name = "PciKeyCompromiseAlert",
                        Subject = "CRITICAL: PCI DSS Key Compromise Alert",
                        HtmlBody = @"
                            <div class='alert alert-critical pci-alert'>
                                <h2>Cryptographic Key Compromise Alert</h2>
                                <p>A potential key compromise has been detected that requires immediate action per PCI DSS requirements:</p>
                            </div>
                            <div class='incident-details'>
                                <h3>Incident Details</h3>
                                <ul>
                                    <li><strong>Detection Time:</strong> {DetectionTime}</li>
                                    <li><strong>Affected Keys:</strong> {AffectedKeys}</li>
                                    <li><strong>Detection Method:</strong> {DetectionMethod}</li>
                                    <li><strong>Risk Level:</strong> {RiskLevel}</li>
                                </ul>
                            </div>
                            <div class='cardholder-impact'>
                                <h3>Cardholder Data Impact</h3>
                                <ul>
                                    <li><strong>Affected Records:</strong> {AffectedRecords}</li>
                                    <li><strong>Data Types:</strong> {DataTypes}</li>
                                    <li><strong>Exposure Window:</strong> {ExposureWindow}</li>
                                </ul>
                            </div>
                            <div class='immediate-actions'>
                                <h3>Required Immediate Actions</h3>
                                <ol>
                                    <li>Revoke compromised keys</li>
                                    <li>Generate new key material</li>
                                    <li>Update all dependent systems</li>
                                    <li>Document incident details</li>
                                    <li>Notify QSA and card brands</li>
                                    <li>Initiate forensic investigation</li>
                                </ol>
                            </div>
                            <div class='incident-team'>
                                <h3>Incident Response Team</h3>
                                <ul>
                                    <li><strong>Security Lead:</strong> {SecurityLead}</li>
                                    <li><strong>Key Custodians:</strong> {KeyCustodians}</li>
                                    <li><strong>Forensics Team:</strong> {ForensicsTeam}</li>
                                </ul>
                            </div>
                            <div class='action-buttons'>
                                <a href='{StartResponseLink}' class='button button-critical'>Start Response</a>
                                <a href='{NotifyQsaLink}' class='button'>Notify QSA</a>
                                <a href='{IncidentLogLink}' class='button'>View Incident Log</a>
                            </div>",
                        NotificationType = NotificationType.PciKeyCompromiseAlert,
                        DefaultPriority = NotificationPriority.Critical,
                        RequiresMFA = true,
                        RequiresAcknowledgment = true,
                        ExpiresAfter = TimeSpan.FromHours(1)
                    }
                },
                {
                    NotificationType.PciKeyUsageAlert,
                    new EmailTemplate
                    {
                        Name = "PciKeyUsageAlert",
                        Subject = "PCI DSS: Key Usage Threshold Alert - {KeyIdentifier}",
                        HtmlBody = @"
                            <div class='alert alert-warning pci-alert'>
                                <h2>Key Usage Threshold Alert</h2>
                                <p>Unusual or threshold-exceeding key usage patterns detected:</p>
                            </div>
                            <div class='usage-details'>
                                <h3>Key Usage Details</h3>
                                <ul>
                                    <li><strong>Key Identifier:</strong> {KeyIdentifier}</li>
                                    <li><strong>Key Type:</strong> {KeyType}</li>
                                    <li><strong>Current Usage Count:</strong> {CurrentUsageCount}</li>
                                    <li><strong>Usage Threshold:</strong> {UsageThreshold}</li>
                                    <li><strong>Usage Period:</strong> {UsagePeriod}</li>
                                    <li><strong>Usage Trend:</strong> {UsageTrend}</li>
                                </ul>
                            </div>
                            <div class='usage-patterns'>
                                <h3>Usage Pattern Analysis</h3>
                                <ul>
                                    <li><strong>Peak Usage Time:</strong> {PeakUsageTime}</li>
                                    <li><strong>Unusual Patterns:</strong> {UnusualPatterns}</li>
                                    <li><strong>System Load:</strong> {SystemLoad}</li>
                                    <li><strong>Error Rate:</strong> {ErrorRate}</li>
                                </ul>
                            </div>
                            <div class='recommendations'>
                                <h3>Recommended Actions</h3>
                                <ol>
                                    <li>Review usage patterns for potential security issues</li>
                                    <li>Verify authorized system access</li>
                                    <li>Check for system performance impact</li>
                                    <li>Consider key rotation if needed</li>
                                    <li>Update monitoring thresholds if necessary</li>
                                </ol>
                            </div>
                            <div class='action-buttons'>
                                <a href='{ViewDetailsLink}' class='button'>View Usage Details</a>
                                <a href='{AdjustThresholdsLink}' class='button'>Adjust Thresholds</a>
                                <a href='{ScheduleRotationLink}' class='button'>Schedule Rotation</a>
                            </div>",
                        NotificationType = NotificationType.PciKeyUsageAlert,
                        DefaultPriority = NotificationPriority.High,
                        RequiresAcknowledgment = true
                    }
                },
                {
                    NotificationType.PciComplianceReport,
                    new EmailTemplate
                    {
                        Name = "PciComplianceReport",
                        Subject = "PCI DSS: Key Management Compliance Report - {ReportingPeriod}",
                        HtmlBody = @"
                            <div class='alert alert-info pci-alert'>
                                <h2>PCI DSS Key Management Compliance Report</h2>
                                <p>Periodic compliance report for cryptographic key management:</p>
                            </div>
                            <div class='report-summary'>
                                <h3>Report Overview</h3>
                                <ul>
                                    <li><strong>Reporting Period:</strong> {ReportingPeriod}</li>
                                    <li><strong>Generation Date:</strong> {GenerationDate}</li>
                                    <li><strong>Overall Compliance Status:</strong> {ComplianceStatus}</li>
                                    <li><strong>Total Findings:</strong> {TotalFindings}</li>
                                </ul>
                            </div>
                            <div class='compliance-metrics'>
                                <h3>Key Management Metrics</h3>
                                <ul>
                                    <li><strong>Keys Within Rotation Policy:</strong> {KeysWithinPolicy}%</li>
                                    <li><strong>Successful Key Operations:</strong> {SuccessfulOperations}%</li>
                                    <li><strong>Failed Operations:</strong> {FailedOperations}</li>
                                    <li><strong>Pending Rotations:</strong> {PendingRotations}</li>
                                    <li><strong>Backup Compliance:</strong> {BackupCompliance}%</li>
                                </ul>
                            </div>
                            <div class='requirement-status'>
                                <h3>PCI DSS Requirement Status</h3>
                                <table class='compliance-table'>
                                    <tr>
                                        <th>Requirement</th>
                                        <th>Status</th>
                                        <th>Last Verified</th>
                                    </tr>
                                    {RequirementStatusRows}
                                </table>
                            </div>
                            <div class='findings'>
                                <h3>Key Findings</h3>
                                <ul>{FindingsList}</ul>
                            </div>
                            <div class='remediation'>
                                <h3>Required Remediation Actions</h3>
                                <ol>{RemediationSteps}</ol>
                            </div>
                            <div class='action-buttons'>
                                <a href='{DownloadReportLink}' class='button'>Download Full Report</a>
                                <a href='{ViewRemediationLink}' class='button'>View Remediation Plan</a>
                                <a href='{ScheduleReviewLink}' class='button'>Schedule Review</a>
                            </div>",
                        NotificationType = NotificationType.PciComplianceReport,
                        DefaultPriority = NotificationPriority.Medium,
                        RequiresAcknowledgment = true
                    }
                },
                {
                    NotificationType.PciKeyCeremonyNotification,
                    new EmailTemplate
                    {
                        Name = "PciKeyCeremonyNotification",
                        Subject = "PCI DSS: Key Ceremony Scheduled - {CeremonyType}",
                        HtmlBody = @"
                            <div class='alert alert-info pci-alert'>
                                <h2>Cryptographic Key Ceremony</h2>
                                <p>A key ceremony has been scheduled in accordance with PCI DSS requirements:</p>
                            </div>
                            <div class='ceremony-details'>
                                <h3>Ceremony Information</h3>
                                <ul>
                                    <li><strong>Ceremony Type:</strong> {CeremonyType}</li>
                                    <li><strong>Date and Time:</strong> {CeremonyDateTime}</li>
                                    <li><strong>Location:</strong> {SecureLocation}</li>
                                    <li><strong>Duration:</strong> {EstimatedDuration}</li>
                                    <li><strong>Key Purpose:</strong> {KeyPurpose}</li>
                                </ul>
                            </div>
                            <div class='required-participants'>
                                <h3>Required Participants</h3>
                                <ul>
                                    <li><strong>Key Custodians:</strong> {KeyCustodians}</li>
                                    <li><strong>Security Officer:</strong> {SecurityOfficer}</li>
                                    <li><strong>Witnesses:</strong> {Witnesses}</li>
                                    <li><strong>Auditor:</strong> {Auditor}</li>
                                </ul>
                            </div>
                            <div class='ceremony-checklist'>
                                <h3>Pre-Ceremony Checklist</h3>
                                <ol>
                                    <li>Review ceremony procedures</li>
                                    <li>Verify HSM availability</li>
                                    <li>Prepare backup materials</li>
                                    <li>Test ceremony equipment</li>
                                    <li>Verify participant credentials</li>
                                    <li>Prepare documentation forms</li>
                                </ol>
                            </div>
                            <div class='security-requirements'>
                                <h3>Security Requirements</h3>
                                <ul>
                                    <li><strong>Required ID:</strong> {RequiredId}</li>
                                    <li><strong>Access Cards:</strong> {AccessCards}</li>
                                    <li><strong>Security Tokens:</strong> {SecurityTokens}</li>
                                    <li><strong>Backup Materials:</strong> {BackupMaterials}</li>
                                </ul>
                            </div>
                            <div class='documentation'>
                                <h3>Required Documentation</h3>
                                <ul>
                                    <li>Ceremony script</li>
                                    <li>Split knowledge forms</li>
                                    <li>Chain of custody records</li>
                                    <li>Key component forms</li>
                                    <li>Witness attestation forms</li>
                                </ul>
                            </div>
                            <div class='action-buttons'>
                                <a href='{ConfirmAttendanceLink}' class='button'>Confirm Attendance</a>
                                <a href='{ViewProcedureLink}' class='button'>View Procedure</a>
                                <a href='{DownloadFormsLink}' class='button'>Download Forms</a>
                            </div>",
                        NotificationType = NotificationType.PciKeyCeremonyNotification,
                        DefaultPriority = NotificationPriority.High,
                        RequiresMFA = true,
                        RequiresAcknowledgment = true
                    }
                }
            };

            return templates;
        }

        public EmailTemplate GetTemplate(NotificationType type)
        {
            if (_templates.TryGetValue(type, out var template))
            {
                return template;
            }

            _logger.LogWarning("Template not found for notification type: {Type}", type);
            throw new KeyNotFoundException($"Email template not found for notification type: {type}");
        }

        public string FormatTemplate(EmailTemplate template, Dictionary<string, string> values)
        {
            var formattedBody = template.HtmlBody;

            foreach (var placeholder in template.Placeholders ?? new Dictionary<string, string>())
            {
                if (values.TryGetValue(placeholder.Key, out var value))
                {
                    formattedBody = formattedBody.Replace($"{{{placeholder.Key}}}", value);
                }
                else
                {
                    _logger.LogWarning("Value not provided for placeholder: {Placeholder}", placeholder.Key);
                }
            }

            return formattedBody;
        }
    }
}
