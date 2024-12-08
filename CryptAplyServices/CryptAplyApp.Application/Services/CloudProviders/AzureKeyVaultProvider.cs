using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Azure.Security.KeyVault.Keys;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Services.CloudProviders
{
    public class AzureKeyVaultProvider : IKeyRotationProvider
    {
        private readonly ILogger<AzureKeyVaultProvider> _logger;
        private readonly Dictionary<string, KeyClient> _keyClients;

        public string ProviderName => "AzureKeyVault";

        public AzureKeyVaultProvider(ILogger<AzureKeyVaultProvider> logger)
        {
            _logger = logger;
            _keyClients = new Dictionary<string, KeyClient>();
        }

        public async Task<RotationCapabilities> GetCapabilitiesAsync()
        {
            return new RotationCapabilities
            {
                SupportsAutomatedRotation = true,
                SupportsCustomKeyMaterial = true,
                SupportsKeyVersioning = true,
                SupportsAsyncRotation = true,
                SupportedKeyTypes = new List<string> { "RSA", "EC" },
                SupportedRotationStrategies = new List<string> 
                { 
                    RotationStrategy.ProviderManaged.ToString(),
                    RotationStrategy.CryptAplyOrchestrated.ToString() 
                },
                RequiredConfigFields = new Dictionary<string, string>
                {
                    { "VaultUri", "The URI of the Azure Key Vault" },
                    { "TenantId", "Azure AD Tenant ID" },
                    { "ClientId", "Service Principal Client ID" },
                    { "ClientSecret", "Service Principal Secret" }
                }
            };
        }

        public async Task<RotationResult> RotateKeyAsync(KeyRotationRequest request)
        {
            try
            {
                var keyClient = await GetKeyClientAsync(request.VaultIdentifier);

                switch (request.Strategy)
                {
                    case RotationStrategy.ProviderManaged:
                        return await HandleProviderManagedRotationAsync(keyClient, request);
                    
                    case RotationStrategy.CryptAplyOrchestrated:
                        return await HandleCryptAplyOrchestratedRotationAsync(keyClient, request);
                    
                    default:
                        throw new NotSupportedException($"Rotation strategy {request.Strategy} is not supported");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to rotate key {KeyName} in vault {VaultId}", 
                    request.KeyName, request.VaultIdentifier);
                throw;
            }
        }

        private async Task<RotationResult> HandleProviderManagedRotationAsync(
            KeyClient keyClient, 
            KeyRotationRequest request)
        {
            // Create new version of the key
            var operation = await keyClient.CreateKeyAsync(request.KeyName, KeyType.Rsa);
            var newKey = operation.Value;

            return new RotationResult
            {
                Success = true,
                NewKeyVersion = newKey.Properties.Version,
                RotationId = Guid.NewGuid().ToString(),
                ProviderMetadata = new Dictionary<string, string>
                {
                    { "KeyId", newKey.Id.ToString() },
                    { "Algorithm", newKey.Key.SignatureAlgorithm.ToString() }
                }
            };
        }

        private async Task<RotationResult> HandleCryptAplyOrchestratedRotationAsync(
            KeyClient keyClient, 
            KeyRotationRequest request)
        {
            // Create new version but let CryptAply handle the orchestration
            var operation = await keyClient.CreateKeyAsync(
                request.KeyName,
                KeyType.Rsa,
                new CreateKeyOptions
                {
                    Enabled = false // Start disabled until orchestration is complete
                }
            );
            var newKey = operation.Value;

            return new RotationResult
            {
                Success = true,
                NewKeyVersion = newKey.Properties.Version,
                RotationId = Guid.NewGuid().ToString(),
                RequiresOrchestration = true,
                ProviderMetadata = new Dictionary<string, string>
                {
                    { "KeyId", newKey.Id.ToString() },
                    { "VaultUri", request.VaultIdentifier }
                }
            };
        }

        public async Task<RotationProgress> GetRotationProgressAsync(string rotationId)
        {
            // Azure Key Vault rotations are immediate, so we just return completed
            return new RotationProgress
            {
                Status = "Completed",
                ProgressPercentage = 100,
                CompletedSteps = new List<string> { "KeyCreation" },
                RemainingSteps = new List<string>()
            };
        }

        public async Task<KeyValidationResult> ValidateKeyConfigurationAsync(KeyConfig config)
        {
            var result = new KeyValidationResult
            {
                IsValid = true,
                ValidationErrors = new List<string>(),
                Warnings = new Dictionary<string, string>()
            };

            try
            {
                // Validate vault credentials
                if (!config.VaultCredentials.ContainsKey("VaultUri"))
                {
                    result.IsValid = false;
                    result.ValidationErrors.Add("VaultUri is required");
                }

                // Test connection
                if (result.IsValid)
                {
                    var keyClient = await GetKeyClientAsync(config.VaultCredentials["VaultUri"]);
                    await keyClient.GetPropertiesOfKeysAsync().AsPages().GetAsyncEnumerator().MoveNextAsync();
                }
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"Failed to validate configuration: {ex.Message}");
            }

            return result;
        }

        private async Task<KeyClient> GetKeyClientAsync(string vaultUri)
        {
            if (!_keyClients.ContainsKey(vaultUri))
            {
                var credential = new DefaultAzureCredential();
                _keyClients[vaultUri] = new KeyClient(new Uri(vaultUri), credential);
            }
            return _keyClients[vaultUri];
        }
    }
}
