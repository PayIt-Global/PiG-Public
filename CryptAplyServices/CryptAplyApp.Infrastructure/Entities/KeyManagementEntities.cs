using System;
using System.Collections.Generic;
using System.Text.Json;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Infrastructure.Entities
{
    public class EncryptionKeyEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] KeyMaterial { get; set; }
        public string Algorithm { get; set; }
        public int KeySize { get; set; }
        public int Version { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastRotatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public KeyStatus Status { get; set; }
        public KeyRotationStatus RotationStatus { get; set; }
        public string CreatedBy { get; set; }
        public string Purpose { get; set; }
        public string TagsJson { get; set; }
        public string MetadataJson { get; set; }

        public List<string> Tags
        {
            get => string.IsNullOrEmpty(TagsJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(TagsJson);
            set => TagsJson = JsonSerializer.Serialize(value);
        }

        public Dictionary<string, string> Metadata
        {
            get => string.IsNullOrEmpty(MetadataJson) ? new Dictionary<string, string>() : JsonSerializer.Deserialize<Dictionary<string, string>>(MetadataJson);
            set => MetadataJson = JsonSerializer.Serialize(value);
        }
    }

    public class KeyRotationEntity
    {
        public string RotationId { get; set; }
        public string KeyId { get; set; }
        public int OldVersion { get; set; }
        public int NewVersion { get; set; }
        public byte[] NewKeyMaterial { get; set; }
        public string InitiatedBy { get; set; }
        public DateTime InitiatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public KeyRotationStatus Status { get; set; }
        public string MetadataJson { get; set; }

        public Dictionary<string, string> Metadata
        {
            get => string.IsNullOrEmpty(MetadataJson) ? new Dictionary<string, string>() : JsonSerializer.Deserialize<Dictionary<string, string>>(MetadataJson);
            set => MetadataJson = JsonSerializer.Serialize(value);
        }

        public virtual EncryptionKeyEntity Key { get; set; }
    }

    public class ArchivedKeyVersionEntity
    {
        public string Id { get; set; }
        public string KeyId { get; set; }
        public int Version { get; set; }
        public byte[] KeyMaterial { get; set; }
        public DateTime ArchivedAt { get; set; }
        public string ArchivedBy { get; set; }
        public string RotationId { get; set; }

        public virtual EncryptionKeyEntity Key { get; set; }
        public virtual KeyRotationEntity Rotation { get; set; }
    }
}
