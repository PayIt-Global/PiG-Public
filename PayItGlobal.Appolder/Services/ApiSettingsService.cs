using Microsoft.Extensions.Options;
using PayItGlobal.App.ConfigurationModels;

namespace PayItGlobal.App.Services
{
    public interface IApiSettingsService
    {
        ApiSettings GetSettings();
    }
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
    }

}
