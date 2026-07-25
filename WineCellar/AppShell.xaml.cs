namespace WineCellar;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        Routing.RegisterRoute(nameof(Views.VinhoDetailPage),typeof(Views.VinhoDetailPage));
        Routing.RegisterRoute(nameof(Views.VinhoFormPage),typeof(Views.VinhoFormPage));
    }
}