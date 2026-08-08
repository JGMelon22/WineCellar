using WineCellar.ViewModels;

namespace WineCellar.Views;

public partial class VinhoDetailPage : ContentPage
{
    private readonly VinhoDetailViewModel _viewModel;

    public VinhoDetailPage(VinhoDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CarregarCommand.ExecuteAsync(null);
    }
}