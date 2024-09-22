using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using PayItGlobalApp.Application.ConfigurationModels;
using PayItGlobalApp.Application.Interfaces;
using PayItGlobalApp.Pages;
using PayItGlobalApp.Services;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace PayItGlobalApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiReactorApp<MainPage>()
                .UseSkiaSharp()
#if DEBUG
                .EnableMauiReactorHotReload()
#endif
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Inter-Regular.ttf", "InterRegular");
                    fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
                    fonts.AddFont("Poppins-Bold.ttf", "PoppinsBold");
                });

            Controls.Native.BorderlessEntry.Configure();

            // Load configuration from appsettings.json
            using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").Result;
            builder.Configuration.AddJsonStream(stream!);

            // Register ApiSettings with the DI container
            builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

            // Register HttpClient
            builder.Services.AddHttpClient();

            // Register your services here
            builder.Services.AddSingleton<IClientTokenService, ClientTokenService>();
            builder.Services.AddSingleton<AuthService>();



#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
