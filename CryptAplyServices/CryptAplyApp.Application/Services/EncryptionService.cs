using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Infrastructure.Repositories;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Services
{
    public interface IEncryptionService
    {
        Task<EncryptionKey> GenerateKeyAsync();
        Task<byte[]> EncryptAsync(byte[] data, string keyId);
        Task<byte[]> DecryptAsync(byte[] encryptedData, string keyId);
        Task<string> EncryptTextAsync(string text, string keyId);
        Task<string> DecryptTextAsync(string encryptedText, string keyId);
    }

    public class EncryptionService : IEncryptionService
    {
        private readonly ILogger<EncryptionService> _logger;
        private readonly IKeyRepository _keyRepository;
        private const int KEY_SIZE = 256;
        private const int BLOCK_SIZE = 128;

        public EncryptionService(
            ILogger<EncryptionService> logger,
            IKeyRepository keyRepository)
        {
            _logger = logger;
            _keyRepository = keyRepository;
        }

        public async Task<EncryptionKey> GenerateKeyAsync()
        {
            try
            {
                using var aes = Aes.Create();
                aes.KeySize = KEY_SIZE;
                aes.BlockSize = BLOCK_SIZE;
                aes.GenerateKey();

                return new EncryptionKey
                {
                    KeyMaterial = aes.Key,
                    Algorithm = "AES",
                    KeySize = KEY_SIZE,
                    CreatedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating encryption key");
                throw;
            }
        }

        public async Task<byte[]> EncryptAsync(byte[] data, string keyId)
        {
            try
            {
                var key = await _keyRepository.GetKeyByIdAsync(keyId);
                if (key == null)
                {
                    throw new KeyNotFoundException($"Key {keyId} not found");
                }

                using var aes = Aes.Create();
                aes.KeySize = KEY_SIZE;
                aes.BlockSize = BLOCK_SIZE;
                aes.Key = key.KeyMaterial;
                aes.GenerateIV();

                using var msEncrypt = new MemoryStream();
                // Write the IV first
                await msEncrypt.WriteAsync(aes.IV, 0, aes.IV.Length);

                using (var encryptor = aes.CreateEncryptor())
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    await csEncrypt.WriteAsync(data, 0, data.Length);
                }

                return msEncrypt.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error encrypting data with key {KeyId}", keyId);
                throw;
            }
        }

        public async Task<byte[]> DecryptAsync(byte[] encryptedData, string keyId)
        {
            try
            {
                var key = await _keyRepository.GetKeyByIdAsync(keyId);
                if (key == null)
                {
                    throw new KeyNotFoundException($"Key {keyId} not found");
                }

                using var aes = Aes.Create();
                aes.KeySize = KEY_SIZE;
                aes.BlockSize = BLOCK_SIZE;
                aes.Key = key.KeyMaterial;

                using var msDecrypt = new MemoryStream(encryptedData);
                // Read the IV first
                byte[] iv = new byte[aes.BlockSize / 8];
                await msDecrypt.ReadAsync(iv, 0, iv.Length);
                aes.IV = iv;

                using var decryptor = aes.CreateDecryptor();
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var msPlain = new MemoryStream();
                await csDecrypt.CopyToAsync(msPlain);

                return msPlain.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error decrypting data with key {KeyId}", keyId);
                throw;
            }
        }

        public async Task<string> EncryptTextAsync(string text, string keyId)
        {
            var data = System.Text.Encoding.UTF8.GetBytes(text);
            var encryptedData = await EncryptAsync(data, keyId);
            return Convert.ToBase64String(encryptedData);
        }

        public async Task<string> DecryptTextAsync(string encryptedText, string keyId)
        {
            var encryptedData = Convert.FromBase64String(encryptedText);
            var decryptedData = await DecryptAsync(encryptedData, keyId);
            return System.Text.Encoding.UTF8.GetString(decryptedData);
        }
    }
}
