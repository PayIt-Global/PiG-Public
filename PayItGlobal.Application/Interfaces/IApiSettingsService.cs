using PayItGlobal.Application.ConfigurationModels;

namespace PayItGlobal.Application.Interfaces
{
    public interface IApiSettingsService
    {
        ApiSettings GetSettings();
        string GetApiBaseUrl();
    }
}
