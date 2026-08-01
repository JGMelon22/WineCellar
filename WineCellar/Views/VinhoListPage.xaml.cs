using WineCellar.Data;
using WineCellar.Models;

namespace WineCellar.Views;

public partial class VinhoListPage : ContentPage
{
    private readonly IVinhoRepositorio _repositorio;

    public VinhoListPage(IVinhoRepositorio repositorio)
    {
        InitializeComponent();
        _repositorio = repositorio;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        VinhosCollectionView.ItemsSource = await _repositorio.ObterTodos();
    }

    private async void OnAdicionarClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(VinhoFormPage));
    }

    private async void OnVinhoSelecionado(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Vinho vinhoSelecionado)
            return;

        VinhosCollectionView.SelectedItem = null; // Limpa seleção visual
        await Shell.Current.GoToAsync($"{nameof(VinhoDetailPage)}?id={vinhoSelecionado.Id}");
    }
}