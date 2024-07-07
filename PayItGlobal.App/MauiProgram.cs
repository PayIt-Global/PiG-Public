using MauiReactor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PayItGlobal.App.ConfigurationModels;
using PayItGlobal.App.Pages;
using System;

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

        // Load configuration
        var assembly = typeof(MauiProgram).Assembly;
        string appSettingsFileName = "appsettings.json";
        using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{appSettingsFileName}");
        if (stream != null)
        {
            var configBuilder = new ConfigurationBuilder()
                .AddJsonStream(stream);
            var configuration = configBuilder.Build();

            // Register configuration
            builder.Configuration.AddConfiguration(configuration);

            // Register services
            builder.Services.AddSingleton<ApiSettings>(sp =>
                sp.GetRequiredService<IConfiguration>().GetSection("ApiSettings").Get<ApiSettings>());
        }
        else
        {
            // Handle the case where the appsettings.json file is not found
            // For example, log an error or throw an exception
            Console.WriteLine("appsettings.json not found.");
        }

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
