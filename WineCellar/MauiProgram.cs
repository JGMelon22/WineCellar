using Microsoft.Extensions.Logging;
using WineCellar.Data;
using WineCellar.Services;
using WineCellar.ViewModels;
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

        builder.Services.AddSingleton<IVinhoRepositorio, VinhoRepositorio>();
        builder.Services.AddSingleton<IFotoService, FotoService>();

        builder.Services.AddTransient<VinhoListPage>();
        builder.Services.AddTransient<VinhoListViewModel>();

        builder.Services.AddTransient<VinhoFormPage>();
        builder.Services.AddTransient<VinhoFormViewModel>();

        builder.Services.AddTransient<VinhoDetailPage>();
        builder.Services.AddTransient<VinhoDetailViewModel>();

        return builder.Build();
    }
}