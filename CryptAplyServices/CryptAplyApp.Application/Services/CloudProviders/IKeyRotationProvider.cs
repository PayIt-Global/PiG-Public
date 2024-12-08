using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Services.CloudProviders
{
    public interface IKeyRotationProvider
    {
        string ProviderName { get; }
        Task<RotationCapabilities> GetCapabilitiesAsync();
        Task<RotationResult> RotateKeyAsync(KeyRotationRequest request);
        Task<RotationProgress> GetRotationProgressAsync(string rotationId);
        Task<KeyValidationResult> ValidateKeyConfigurationAsync(KeyConfig config);
    }

    public class KeyRotationRequest
    {
        public string KeyId { get; set; }
        public string KeyName { get; set; }
        public string VaultIdentifier { get; set; }
        public Dictionary<string, string> ProviderSpecificConfig { get; set; }
        public RotationStrategy Strategy { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

    public class RotationCapabilities
    {
        public bool SupportsAutomatedRotation { get; set; }
        public bool SupportsCustomKeyMaterial { get; set; }
        public bool SupportsKeyVersioning { get; set; }
        public bool SupportsAsyncRotation { get; set; }
        public List<string> SupportedKeyTypes { get; set; }
        public List<string> SupportedRotationStrategies { get; set; }
        public Dictionary<string, string> RequiredConfigFields { get; set; }
    }

    public class KeyValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> ValidationErrors { get; set; }
        public Dictionary<string, string> Warnings { get; set; }
    }

    public class KeyConfig
    {
        public string KeyType { get; set; }
        public string VaultType { get; set; }
        public Dictionary<string, string> VaultCredentials { get; set; }
        public Dictionary<string, string> KeyAttributes { get; set; }
    }

    public enum RotationStrategy
    {
        /// <summary>
        /// Provider handles the entire rotation process
        /// </summary>
        ProviderManaged,

        /// <summary>
        /// CryptAply orchestrates the rotation but provider executes key operations
        /// </summary>
        CryptAplyOrchestrated,

        /// <summary>
        /// Client handles rotation with provider's guidance
        /// </summary>
        ClientManaged
    }
}
