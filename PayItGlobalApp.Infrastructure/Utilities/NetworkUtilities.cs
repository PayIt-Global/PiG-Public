namespace PayItGlobalApp.Application.Infrastructure.Utilities
{
    public static class NetworkUtilities
    {
        public static async Task<string> GetUserIpAddressAsync()
        {
            using (var httpClient = new HttpClient())
            {
                // Example using a public IP address API
                string publicIpApiUrl = "https://api.ipify.org";
                try
                {
                    string ipAddress = await httpClient.GetStringAsync(publicIpApiUrl);
                    return ipAddress;
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., network issues)
                    return null;
                }
            }
        }
    }
}
