using WineCellar.ViewModels;

namespace WineCellar.Views;

public partial class VinhoListPage : ContentPage
{
    private readonly VinhoListViewModel _viewModel;

    public VinhoListPage(VinhoListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CarregarVinhosCommand.ExecuteAsync(null);
    }
}