using WineCellar.ViewModels;

namespace WineCellar.Views;

public partial class VinhoFormPage : ContentPage
{
    private readonly VinhoFormViewModel _viewModel;

    public VinhoFormPage(VinhoFormViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CarregarCommand.ExecuteAsync(null);
    }
}