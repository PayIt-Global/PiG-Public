using Microsoft.Extensions.Options;
using PayItGlobal.Application.ConfigurationModels;
using PayItGlobal.Application.Interfaces;

namespace PayItGlobal.Application.Services
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
