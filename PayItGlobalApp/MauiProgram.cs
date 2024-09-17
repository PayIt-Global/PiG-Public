using MauiReactor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PayItGlobalApp.Pages;
using SkiaSharp.Views.Maui.Controls.Hosting;
using PayItGlobalApp.Pages.Components;
using PayItGlobal.Application.ConfigurationModels;
using PayItGlobal.Application.Interfaces;
using PayItGlobal.Application.Services;
using PayItGlobal.Domain.Interfaces;
using PayItGlobal.Infrastructure.Repository;
using PayItGlobal.Infrastructure.Services;
using System;
using System.IO;
using System.Reflection;
using Microsoft.Maui.Storage;

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
}
