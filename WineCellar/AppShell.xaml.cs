using WineCellar.Views;

namespace WineCellar;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(VinhoDetailPage), typeof(VinhoDetailPage));
        Routing.RegisterRoute(nameof(VinhoFormPage), typeof(VinhoFormPage));
    }
}