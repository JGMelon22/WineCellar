using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WineCellar.Data;
using WineCellar.Models;
using WineCellar.Views;

namespace WineCellar.ViewModels;

public partial class VinhoListViewModel(IVinhoRepositorio repositorio) : ObservableObject
{
    [ObservableProperty] private bool _estaCarregando;

    [ObservableProperty] private ObservableCollection<Vinho> _vinhos = new();

    [RelayCommand]
    private async Task CarregarVinhosAsync()
    {
        if (EstaCarregando)
            return;

        EstaCarregando = true;

        var lista = await repositorio.ObterTodos();
        Vinhos = new ObservableCollection<Vinho>(lista);

        EstaCarregando = false;
    }

    [RelayCommand]
    private async Task AbrirDetalhesAsync(Vinho vinho)
    {
        if (vinho is null)
            return;

        await Shell.Current.GoToAsync($"{nameof(VinhoDetailPage)}?id={vinho.Id}");
    }

    [RelayCommand]
    private async Task NovoVinhoAsync()
    {
        await Shell.Current.GoToAsync(nameof(VinhoFormPage));
    }
}