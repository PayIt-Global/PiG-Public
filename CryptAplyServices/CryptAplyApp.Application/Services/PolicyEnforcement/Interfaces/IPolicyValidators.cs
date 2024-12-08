using System.Threading.Tasks;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Services.PolicyEnforcement
{
    public interface ITimeWindowValidator
    {
        Task<bool> IsWithinAllowedWindowAsync(KeyOperationContext context, TimeWindowControls controls);
        Task<bool> ValidateEmergencyWindowAsync(KeyOperationContext context, EmergencyAccess controls);
    }

    public interface IQuotaValidator
    {
        Task<bool> CheckQuotasAsync(KeyOperationContext context, UsageQuotas controls);
        Task<QuotaStatus> GetCurrentQuotaStatusAsync(string applicationId);
        Task RecordOperationAsync(KeyOperationContext context);
    }

    public interface INetworkValidator
    {
        Task<bool> ValidateNetworkAccessAsync(KeyOperationContext context, NetworkPolicy controls);
        Task<bool> ValidatePrivateEndpointAsync(string sourceIp, string[] allowedVnets);
        Task<bool> IsInAllowedSubnetAsync(string sourceIp, string[] allowedSubnets);
    }

    public interface IAuditLogger
    {
        Task LogAsync(AuditEvent auditEvent);
        Task<AuditEvent[]> GetAuditTrailAsync(string keyId, string timeRange);
        Task<bool> VerifyAuditLogIntegrityAsync(string timeRange);
    }
}
