using Microsoft.Extensions.Options;
using CryptAplyApp.Application.ConfigurationModels;
using CryptAplyApp.Application.Interfaces;

namespace CryptAplyApp.Application.Services
{
    public class ApiSettingsService : IApiSettingsService
    {
        private readonly ApiSettings _apiSettings;

        public ApiSettingsService(IOptions<ApiSettings> options)
        {
            _apiSettings = options.Value;
        }

        public ApiSettings GetSettings()
        {
            return _apiSettings;
        }

        // Method to get the API Base URL from the ApiSettings
        public string GetApiBaseUrl()
        {
            return _apiSettings.BaseUrl;
        }
    }
}
