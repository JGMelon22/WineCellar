using Microsoft.Extensions.Logging;
using WineCellar.Data;
using WineCellar.Views;

namespace WineCellar;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<VinhoRepositorioMemoria>();

        builder.Services.AddTransient<VinhoListPage>();
        builder.Services.AddTransient<VinhoFormPage>();
        builder.Services.AddTransient<VinhoDetailPage>();

        return builder.Build();
    }
}