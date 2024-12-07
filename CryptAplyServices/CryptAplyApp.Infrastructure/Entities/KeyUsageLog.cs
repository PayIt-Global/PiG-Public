using System;

namespace CryptAplyApp.Infrastructure.Entities
{
    public class KeyUsageLog
    {
        public int Id { get; set; }
        public int CryptoKeyId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Operation { get; set; }  // Encrypt, Decrypt, Sign, Verify
        public string UserId { get; set; }
        public string Application { get; set; }
        public string IpAddress { get; set; }
        public bool WasSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public string AdditionalData { get; set; }

        // Navigation property
        public virtual CryptoKey CryptoKey { get; set; }
    }
}
