using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Services.PolicyEnforcement.Validators
{
    public class NetworkValidator : INetworkValidator
    {
        private readonly ILogger<NetworkValidator> _logger;
        private readonly IAuditLogger _auditLogger;
        private readonly Dictionary<string, IPNetwork> _vnetCache;
        private readonly Dictionary<string, IPNetwork> _subnetCache;

        public NetworkValidator(ILogger<NetworkValidator> logger, IAuditLogger auditLogger)
        {
            _logger = logger;
            _auditLogger = auditLogger;
            _vnetCache = new Dictionary<string, IPNetwork>();
            _subnetCache = new Dictionary<string, IPNetwork>();
        }

        public async Task<bool> ValidateNetworkAccessAsync(KeyOperationContext context, NetworkPolicy controls)
        {
            try
            {
                // Check if private endpoint is required
                if (controls.RequirePrivateEndpoint)
                {
                    var isPrivate = await ValidatePrivateEndpointAsync(context.SourceIp, controls.AllowedVnetIds);
                    if (!isPrivate)
                    {
                        await LogNetworkViolationAsync(context, "Private endpoint required but connection is public");
                        return false;
                    }
                }

                // Check if internet access is allowed
                if (!controls.AllowInternetAccess && IsPublicIp(context.SourceIp))
                {
                    await LogNetworkViolationAsync(context, "Internet access not allowed");
                    return false;
                }

                // Check subnet restrictions
                if (controls.AllowedSubnetIds?.Any() == true)
                {
                    var isInAllowedSubnet = await IsInAllowedSubnetAsync(context.SourceIp, controls.AllowedSubnetIds);
                    if (!isInAllowedSubnet)
                    {
                        await LogNetworkViolationAsync(context, "Source IP not in allowed subnets");
                        return false;
                    }
                }

                // Check network tags if specified
                if (controls.RequiredNetworkTags?.Any() == true)
                {
                    var hasRequiredTags = await ValidateNetworkTagsAsync(context.SourceIp, controls.RequiredNetworkTags);
                    if (!hasRequiredTags)
                    {
                        await LogNetworkViolationAsync(context, "Missing required network tags");
                        return false;
                    }
                }

                await _auditLogger.LogAsync(new AuditEvent
                {
                    EventType = "NetworkValidation",
                    ApplicationId = context.ApplicationId,
                    Success = true,
                    Timestamp = DateTime.UtcNow,
                    Metadata = new Dictionary<string, string>
                    {
                        { "SourceIp", context.SourceIp },
                        { "IsPrivate", (!IsPublicIp(context.SourceIp)).ToString() }
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating network access for {ApplicationId}", context.ApplicationId);
                return false;
            }
        }

        public async Task<bool> ValidatePrivateEndpointAsync(string sourceIp, string[] allowedVnets)
        {
            if (string.IsNullOrEmpty(sourceIp) || allowedVnets == null)
            {
                return false;
            }

            try
            {
                var ipAddress = IPAddress.Parse(sourceIp);

                // Check if IP is in any allowed VNet
                foreach (var vnetId in allowedVnets)
                {
                    if (!_vnetCache.TryGetValue(vnetId, out var vnetNetwork))
                    {
                        vnetNetwork = await GetVNetNetworkAsync(vnetId);
                        _vnetCache[vnetId] = vnetNetwork;
                    }

                    if (IsIpInNetwork(ipAddress, vnetNetwork))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating private endpoint for IP {SourceIp}", sourceIp);
                return false;
            }
        }

        public async Task<bool> IsInAllowedSubnetAsync(string sourceIp, string[] allowedSubnets)
        {
            if (string.IsNullOrEmpty(sourceIp) || allowedSubnets == null)
            {
                return false;
            }

            try
            {
                var ipAddress = IPAddress.Parse(sourceIp);

                // Check if IP is in any allowed subnet
                foreach (var subnetId in allowedSubnets)
                {
                    if (!_subnetCache.TryGetValue(subnetId, out var subnetNetwork))
                    {
                        subnetNetwork = await GetSubnetNetworkAsync(subnetId);
                        _subnetCache[subnetId] = subnetNetwork;
                    }

                    if (IsIpInNetwork(ipAddress, subnetNetwork))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating subnet for IP {SourceIp}", sourceIp);
                return false;
            }
        }

        private bool IsPublicIp(string ipAddress)
        {
            if (IPAddress.TryParse(ipAddress, out var ip))
            {
                // Check if IP is in private ranges
                if (ip.GetAddressBytes()[0] == 10) return false; // 10.0.0.0/8
                if (ip.GetAddressBytes()[0] == 172 && (ip.GetAddressBytes()[1] >= 16 && ip.GetAddressBytes()[1] <= 31)) return false; // 172.16.0.0/12
                if (ip.GetAddressBytes()[0] == 192 && ip.GetAddressBytes()[1] == 168) return false; // 192.168.0.0/16

                return true;
            }

            return false;
        }

        private async Task<IPNetwork> GetVNetNetworkAsync(string vnetId)
        {
            // In real implementation, this would fetch VNet CIDR from Azure/AWS/GCP
            // For now, return a dummy network
            return new IPNetwork(IPAddress.Parse("10.0.0.0"), 16);
        }

        private async Task<IPNetwork> GetSubnetNetworkAsync(string subnetId)
        {
            // In real implementation, this would fetch subnet CIDR from Azure/AWS/GCP
            // For now, return a dummy network
            return new IPNetwork(IPAddress.Parse("10.0.1.0"), 24);
        }

        private bool IsIpInNetwork(IPAddress ip, IPNetwork network)
        {
            var ipBytes = ip.GetAddressBytes();
            var networkBytes = network.Network.GetAddressBytes();
            var maskBytes = network.GetSubnetMask().GetAddressBytes();

            for (var i = 0; i < ipBytes.Length; i++)
            {
                if ((ipBytes[i] & maskBytes[i]) != (networkBytes[i] & maskBytes[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> ValidateNetworkTagsAsync(string sourceIp, Dictionary<string, string[]> requiredTags)
        {
            // In real implementation, this would fetch network tags from cloud provider
            // For now, return true
            return true;
        }

        private async Task LogNetworkViolationAsync(KeyOperationContext context, string reason)
        {
            await _auditLogger.LogAsync(new AuditEvent
            {
                EventType = "NetworkViolation",
                ApplicationId = context.ApplicationId,
                Success = false,
                Timestamp = DateTime.UtcNow,
                Details = new[] { reason },
                Metadata = new Dictionary<string, string>
                {
                    { "SourceIp", context.SourceIp }
                }
            });
        }
    }

    public class IPNetwork
    {
        public IPAddress Network { get; }
        private readonly int _prefixLength;

        public IPNetwork(IPAddress network, int prefixLength)
        {
            Network = network;
            _prefixLength = prefixLength;
        }

        public IPAddress GetSubnetMask()
        {
            var maskBytes = new byte[4];
            for (var i = 0; i < 4; i++)
            {
                if (_prefixLength >= 8 * (i + 1))
                    maskBytes[i] = 255;
                else if (_prefixLength <= 8 * i)
                    maskBytes[i] = 0;
                else
                {
                    var bits = _prefixLength - (8 * i);
                    maskBytes[i] = (byte)(255 << (8 - bits));
                }
            }
            return new IPAddress(maskBytes);
        }
    }
}
