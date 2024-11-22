using CryptAplyApp.Application.ConfigurationModels;

namespace CryptAplyApp.Application.Interfaces
{
    public interface IApiSettingsService
    {
        ApiSettings GetSettings();
        string GetApiBaseUrl();
    }
}
