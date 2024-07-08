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
namespace PayItGlobal.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiReactorApp<MainPage>(app =>
            {
                app.AddResource("Resources/Styles/Colors.xaml");
                app.AddResource("Resources/Styles/Styles.xaml");
            })
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

        builder.Services.AddSingleton<ITokenService, TokenService>();
        builder.Services.AddSingleton<IRefreshTokenService, RefreshTokenService>();

        builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>((services) =>
        {
            var httpClient = services.GetRequiredService<HttpClient>();
            var configuration = services.GetRequiredService<IConfiguration>();
            var tokenService = services.GetRequiredService<ITokenService>();
            var refreshTokenService = services.GetRequiredService<IRefreshTokenService>(); // Include this only if your AuthenticationService needs it
            return new AuthenticationService(httpClient, configuration, tokenService, refreshTokenService);
        });


#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

}
