using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CryptAplyApp.Application.Interfaces;
using CryptAplyApp.Application.Models;
using CryptAplyApp.Infrastructure.Data;
using CryptAplyApp.Infrastructure.Entities;

namespace CryptAplyApp.Infrastructure.Repositories
{
    public class KeyRepository : IKeyRepository
    {
        private readonly PciComplianceDbContext _dbContext;

        public KeyRepository(PciComplianceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EncryptionKey> GetKeyByIdAsync(string keyId)
        {
            var entity = await _dbContext.EncryptionKeys.FindAsync(keyId);
            return entity != null ? MapToKey(entity) : null;
        }

        public async Task<List<EncryptionKey>> GetAllKeysAsync()
        {
            var entities = await _dbContext.EncryptionKeys
                .OrderBy(k => k.Name)
                .ToListAsync();

            return entities.Select(MapToKey).ToList();
        }

        public async Task<List<EncryptionKey>> GetKeysByPurposeAsync(string purpose)
        {
            var entities = await _dbContext.EncryptionKeys
                .Where(k => k.Purpose == purpose)
                .OrderBy(k => k.Name)
                .ToListAsync();

            return entities.Select(MapToKey).ToList();
        }

        public async Task<List<EncryptionKey>> GetExpiredKeysAsync()
        {
            var entities = await _dbContext.EncryptionKeys
                .Where(k => k.ExpiresAt.HasValue && k.ExpiresAt.Value <= DateTime.UtcNow)
                .OrderBy(k => k.ExpiresAt)
                .ToListAsync();

            return entities.Select(MapToKey).ToList();
        }

        public async Task<string> CreateKeyAsync(EncryptionKey key)
        {
            var entity = new EncryptionKeyEntity
            {
                Id = string.IsNullOrEmpty(key.Id) ? Guid.NewGuid().ToString() : key.Id,
                Name = key.Name,
                Description = key.Description,
                KeyMaterial = key.KeyMaterial,
                Algorithm = key.Algorithm,
                KeySize = key.KeySize,
                Version = key.Version,
                CreatedAt = key.CreatedAt,
                LastRotatedAt = key.LastRotatedAt,
                ExpiresAt = key.ExpiresAt,
                Status = key.Status,
                RotationStatus = key.RotationStatus,
                CreatedBy = key.CreatedBy,
                Purpose = key.Purpose,
                Tags = key.Tags,
                Metadata = key.Metadata
            };

            _dbContext.EncryptionKeys.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task UpdateKeyAsync(EncryptionKey key)
        {
            var entity = await _dbContext.EncryptionKeys.FindAsync(key.Id);
            if (entity == null)
                throw new KeyNotFoundException($"Key {key.Id} not found");

            entity.Name = key.Name;
            entity.Description = key.Description;
            entity.KeyMaterial = key.KeyMaterial;
            entity.Algorithm = key.Algorithm;
            entity.KeySize = key.KeySize;
            entity.Version = key.Version;
            entity.LastRotatedAt = key.LastRotatedAt;
            entity.ExpiresAt = key.ExpiresAt;
            entity.Status = key.Status;
            entity.RotationStatus = key.RotationStatus;
            entity.Purpose = key.Purpose;
            entity.Tags = key.Tags;
            entity.Metadata = key.Metadata;

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteKeyAsync(string keyId)
        {
            var entity = await _dbContext.EncryptionKeys.FindAsync(keyId);
            if (entity != null)
            {
                _dbContext.EncryptionKeys.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<KeyRotation> GetActiveRotationAsync(string keyId)
        {
            var entity = await _dbContext.KeyRotations
                .FirstOrDefaultAsync(r => r.KeyId == keyId && r.Status == KeyRotationStatus.InProgress);

            return entity != null ? MapToRotation(entity) : null;
        }

        public async Task<List<KeyRotation>> GetKeyRotationHistoryAsync(string keyId)
        {
            var entities = await _dbContext.KeyRotations
                .Where(r => r.KeyId == keyId)
                .OrderByDescending(r => r.InitiatedAt)
                .ToListAsync();

            return entities.Select(MapToRotation).ToList();
        }

        public async Task<string> CreateKeyRotationAsync(KeyRotation rotation)
        {
            var entity = new KeyRotationEntity
            {
                RotationId = string.IsNullOrEmpty(rotation.RotationId) ? Guid.NewGuid().ToString() : rotation.RotationId,
                KeyId = rotation.KeyId,
                OldVersion = rotation.OldVersion,
                NewVersion = rotation.NewVersion,
                NewKeyMaterial = rotation.NewKeyMaterial,
                InitiatedBy = rotation.InitiatedBy,
                InitiatedAt = rotation.InitiatedAt,
                CompletedAt = rotation.CompletedAt,
                Status = rotation.Status,
                Metadata = rotation.Metadata
            };

            _dbContext.KeyRotations.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity.RotationId;
        }

        public async Task UpdateKeyRotationAsync(KeyRotation rotation)
        {
            var entity = await _dbContext.KeyRotations.FindAsync(rotation.RotationId);
            if (entity == null)
                throw new KeyNotFoundException($"Rotation {rotation.RotationId} not found");

            entity.Status = rotation.Status;
            entity.CompletedAt = rotation.CompletedAt;
            entity.Metadata = rotation.Metadata;

            await _dbContext.SaveChangesAsync();
        }

        public async Task ArchiveKeyVersionAsync(string keyId, int version, byte[] keyMaterial)
        {
            var entity = new ArchivedKeyVersionEntity
            {
                Id = Guid.NewGuid().ToString(),
                KeyId = keyId,
                Version = version,
                KeyMaterial = keyMaterial,
                ArchivedAt = DateTime.UtcNow
            };

            _dbContext.ArchivedKeyVersions.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<ArchivedKeyVersion>> GetArchivedVersionsAsync(string keyId)
        {
            var entities = await _dbContext.ArchivedKeyVersions
                .Where(v => v.KeyId == keyId)
                .OrderByDescending(v => v.Version)
                .ToListAsync();

            return entities.Select(MapToArchivedVersion).ToList();
        }

        public async Task<ArchivedKeyVersion> GetArchivedVersionAsync(string keyId, int version)
        {
            var entity = await _dbContext.ArchivedKeyVersions
                .FirstOrDefaultAsync(v => v.KeyId == keyId && v.Version == version);

            return entity != null ? MapToArchivedVersion(entity) : null;
        }

        public async Task<List<EncryptionKey>> GetKeysNeedingRotationAsync(TimeSpan rotationAge)
        {
            var cutoffDate = DateTime.UtcNow.Subtract(rotationAge);
            var entities = await _dbContext.EncryptionKeys
                .Where(k => k.LastRotatedAt <= cutoffDate || k.LastRotatedAt == null)
                .OrderBy(k => k.LastRotatedAt)
                .ToListAsync();

            return entities.Select(MapToKey).ToList();
        }

        private static EncryptionKey MapToKey(EncryptionKeyEntity entity)
        {
            return new EncryptionKey
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                KeyMaterial = entity.KeyMaterial,
                Algorithm = entity.Algorithm,
                KeySize = entity.KeySize,
                Version = entity.Version,
                CreatedAt = entity.CreatedAt,
                LastRotatedAt = entity.LastRotatedAt,
                ExpiresAt = entity.ExpiresAt,
                Status = entity.Status,
                RotationStatus = entity.RotationStatus,
                CreatedBy = entity.CreatedBy,
                Purpose = entity.Purpose,
                Tags = entity.Tags,
                Metadata = entity.Metadata
            };
        }

        private static KeyRotation MapToRotation(KeyRotationEntity entity)
        {
            return new KeyRotation
            {
                RotationId = entity.RotationId,
                KeyId = entity.KeyId,
                OldVersion = entity.OldVersion,
                NewVersion = entity.NewVersion,
                NewKeyMaterial = entity.NewKeyMaterial,
                InitiatedBy = entity.InitiatedBy,
                InitiatedAt = entity.InitiatedAt,
                CompletedAt = entity.CompletedAt,
                Status = entity.Status,
                Metadata = entity.Metadata
            };
        }

        private static ArchivedKeyVersion MapToArchivedVersion(ArchivedKeyVersionEntity entity)
        {
            return new ArchivedKeyVersion
            {
                Id = entity.Id,
                KeyId = entity.KeyId,
                Version = entity.Version,
                KeyMaterial = entity.KeyMaterial,
                ArchivedAt = entity.ArchivedAt,
                ArchivedBy = entity.ArchivedBy,
                RotationId = entity.RotationId
            };
        }
    }
}
