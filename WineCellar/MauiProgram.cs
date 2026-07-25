using Microsoft.Extensions.Logging;
using WineCellar.Data;

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

        builder.Services.AddTransient<Views.VinhoListPage>();
        
        return builder.Build();
    }
}