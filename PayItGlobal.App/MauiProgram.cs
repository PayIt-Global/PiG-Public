using MauiReactor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PayItGlobal.App.ConfigurationModels;
using PayItGlobal.App.Pages;
using System;
using System.IO;
using PayItGlobal.Application.Interfaces;
using PayItGlobal.Application.Services;
using System.Net.Http;
using PayItGlobal.Infrastructure.Services;
using PayItGlobal.Domain.Interfaces;
using PayItGlobal.Infrastructure.Repository;
using PayItGlobal.App.Services;

namespace PayItGlobal.App;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiReactorApp<MainPage>()
#if DEBUG
            .EnableMauiReactorHotReload()
#endif
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
            });

        // Load configuration from appsettings.json
        string appSettingsFileName = "appsettings.json";
        var appSettingsFullPath = Path.Combine(AppContext.BaseDirectory, appSettingsFileName);
        builder.Configuration.AddJsonFile(appSettingsFullPath, optional: true, reloadOnChange: true);

        // Register ApiSettings with the DI container
        builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

        // Register HttpClient
        builder.Services.AddHttpClient();

        // Register your services here
        builder.Services.AddSingleton<ITokenService, TokenService>();
        builder.Services.AddSingleton<IRefreshTokenService, RefreshTokenService>();
        builder.Services.AddSingleton<IRefreshTokenRepository, RefreshTokenRepository>(); 

        // Register AuthenticationService with all required dependencies
        builder.Services.AddSingleton<IClientAuthenticationService, ClientAuthenticationService>();
        builder.Services.AddSingleton<IApiSettingsService, ApiSettingsService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

}
