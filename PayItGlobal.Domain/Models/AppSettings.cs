namespace PayItGlobalApi.Domain.Models
{
    public class AppSettings
    {
        public required string Secret { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        // Add other configuration properties as needed
    }
}
