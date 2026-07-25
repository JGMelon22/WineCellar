using WineCellar.Data;
using WineCellar.Models;

namespace WineCellar.Views;

public partial class VinhoListPage : ContentPage
{
    private readonly VinhoRepositorioMemoria _repositorio;

    public VinhoListPage(VinhoRepositorioMemoria repositorio)
    {
        InitializeComponent();
        _repositorio = repositorio;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        VinhosCollectionView.ItemsSource = _repositorio.ObterTodos().ToList();
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
        await Shell.Current.GoToAsync($"{nameof(VinhoFormPage)}?id={vinhoSelecionado}");
    }

    private async void OnExcluirSwipedEvoked(object? sender, EventArgs e)
    {
        if (sender is not SwipeItem swipeItem || swipeItem.CommandParameter is not int vinhoId)
            return;

        var vinho = _repositorio.ObterVinhoPorId(vinhoId);
        if (vinho is null)
            return;

        var confirmar = await DisplayAlertAsync(
            "Excluir vinho",
            $"Tem certeza que deseja excluir \"{vinho.Nome}\"?",
            "Excluir",
            "Cancelar"
        );

        if (!confirmar)
            return;

        _repositorio.Excluir(vinhoId);
        VinhosCollectionView.ItemsSource = _repositorio.ObterTodos().ToList();
    }
}