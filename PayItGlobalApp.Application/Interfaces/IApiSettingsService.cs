using PayItGlobalApp.Application.ConfigurationModels;

namespace PayItGlobalApp.Application.Interfaces
{
    public interface IApiSettingsService
    {
        ApiSettings GetSettings();
        string GetApiBaseUrl();
    }
}
