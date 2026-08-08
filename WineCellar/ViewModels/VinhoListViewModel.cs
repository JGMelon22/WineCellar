using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WineCellar.Data;
using WineCellar.Models;
using WineCellar.Views;

namespace WineCellar.ViewModels;

public partial class VinhoListViewModel : ObservableObject
{
    private readonly IVinhoRepositorio _repositorio;

    [ObservableProperty] private ObservableCollection<Vinho> _vinhos = new();

    [ObservableProperty] private bool _estaCarregando;

    public VinhoListViewModel(IVinhoRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    [RelayCommand]
    private async Task CarregarVinhosAsync()
    {
        if (EstaCarregando)
            return;

        EstaCarregando = true;

        var lista = await _repositorio.ObterTodos();
        Vinhos = new ObservableCollection<Vinho>(lista);

        EstaCarregando = false;
    }

    [RelayCommand]
    private async Task AbrirDetalhes(Vinho vinho)
    {
        if (vinho is null)
            return;

        await Shell.Current.GoToAsync($"{nameof(VinhoDetailPage)}?id={vinho.Id}");
    }

    [RelayCommand]
    private async Task NovoVinho()
    {
        await Shell.Current.GoToAsync(nameof(VinhoFormPage));
    }
}