using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Interfaces
{
    public interface IKeyRepository
    {
        Task<EncryptionKey> GetKeyByIdAsync(string keyId);
        Task<List<EncryptionKey>> GetAllKeysAsync();
        Task<List<EncryptionKey>> GetKeysByPurposeAsync(string purpose);
        Task<List<EncryptionKey>> GetExpiredKeysAsync();
        Task<string> CreateKeyAsync(EncryptionKey key);
        Task UpdateKeyAsync(EncryptionKey key);
        Task DeleteKeyAsync(string keyId);
        
        Task<KeyRotation> GetActiveRotationAsync(string keyId);
        Task<List<KeyRotation>> GetKeyRotationHistoryAsync(string keyId);
        Task<string> CreateKeyRotationAsync(KeyRotation rotation);
        Task UpdateKeyRotationAsync(KeyRotation rotation);
        
        Task ArchiveKeyVersionAsync(string keyId, int version, byte[] keyMaterial);
        Task<List<ArchivedKeyVersion>> GetArchivedVersionsAsync(string keyId);
        Task<ArchivedKeyVersion> GetArchivedVersionAsync(string keyId, int version);
        
        Task<List<EncryptionKey>> GetKeysNeedingRotationAsync(TimeSpan rotationAge);
    }
}
