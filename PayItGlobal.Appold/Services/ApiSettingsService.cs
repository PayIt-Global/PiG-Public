using Microsoft.Extensions.Options;
using PayItGlobal.Appold.ConfigurationModels;

namespace PayItGlobal.Appold.Services
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
