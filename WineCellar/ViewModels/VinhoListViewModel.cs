using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WineCellar.Data;
using WineCellar.Models;
using WineCellar.Views;

namespace WineCellar.ViewModels;

public partial class VinhoListViewModel(IVinhoRepositorio repositorio) : ObservableObject
{
    private const string OpcaoTodos = "Todos";
    private const int DebounceMs = 400;

    private CancellationTokenSource? _debounceCts;

    [ObservableProperty] private bool _estaCarregando;

    [ObservableProperty] private ObservableCollection<Vinho> _vinhos = new();

    [ObservableProperty] private string _filtroNome = string.Empty;

    [ObservableProperty] private ObservableCollection<string> _tiposDisponiveis = new(
        new[] { OpcaoTodos }.Concat(Enum.GetNames(typeof(TipoVinho)))
    );

    [ObservableProperty] private string _tipoSelecionado = OpcaoTodos;

    [ObservableProperty] private ObservableCollection<string> _paisesDisponiveis = new();

    [ObservableProperty] private string _paisSelecionado = OpcaoTodos;

    [ObservableProperty] private CampoOrdenacao _campoOrdenacaoSelecionado = CampoOrdenacao.Nome;

    [ObservableProperty] private bool _ordemCrescente = true;

    [RelayCommand]
    private async Task CarregarVinhosAsync()
    {
        if (EstaCarregando)
            return;

        EstaCarregando = true;

        var paises = await repositorio.ObterPaisesDistintos();
        PaisesDisponiveis = new ObservableCollection<string>(
            new[] { OpcaoTodos }.Concat(paises));

        EstaCarregando = false;

        await BuscarAsync();
    }

    [RelayCommand]
    private async Task AbrirOrdenacaoAsync()
    {
        var opcoes = new Dictionary<string, (CampoOrdenacao Campo, bool Crescente)>
        {
            ["Nome (A-Z)"] = (CampoOrdenacao.Nome, true),
            ["Nome (Z-A)"] = (CampoOrdenacao.Nome, false),
            ["Ano (mais antigo primeiro)"] = (CampoOrdenacao.Ano, true),
            ["Ano (mais recente primeiro)"] = (CampoOrdenacao.Ano, false),
            ["Nota (menor primeiro)"] = (CampoOrdenacao.Nota, true),
            ["Nota (maior primeiro)"] = (CampoOrdenacao.Nota, false),
        };

        var escolha = await Shell.Current.DisplayActionSheetAsync(
            "Ordernar pro", "Cancelar", null, opcoes.Keys.ToArray());

        if (escolha is null || escolha == "Cancelar" || !opcoes.TryGetValue(escolha, out var selecionado))
            return;

        CampoOrdenacaoSelecionado = selecionado.Campo;
        OrdemCrescente = selecionado.Crescente;

        await BuscarAsync();
    }

    [RelayCommand]
    private async Task BuscarAsync()
    {
        EstaCarregando = true;

        TipoVinho? tipoVinho = TipoSelecionado == OpcaoTodos
            ? null
            : Enum.Parse<TipoVinho>(TipoSelecionado);

        string? pais = PaisSelecionado == OpcaoTodos
            ? null
            : PaisSelecionado;

        var lista = await repositorio.Buscar(
            FiltroNome, tipoVinho, pais, CampoOrdenacaoSelecionado, OrdemCrescente);
        Vinhos = new ObservableCollection<Vinho>(lista);

        EstaCarregando = false;
    }

    // // Disparado automaticamente pelo CommunityToolkit.Mvvm sempre que FiltroNome muda
    partial void OnFiltroNomeChanged(string value) => AgendarBuscaComDebounce();

    // Picker não precisa de debounce: cada seleção já é uma escolha
    // deliberada do usuário, não uma tecla no meio de uma palavra.

    partial void OnTipoSelecionadoChanged(string value) => BuscarCommand.Execute(null);

    partial void OnPaisSelecionadoChanged(string value) => BuscarCommand.Execute(null);

    private void AgendarBuscaComDebounce()
    {
        _debounceCts?.Cancel();
        _debounceCts = new CancellationTokenSource();
        var token = _debounceCts.Token;

        _ = DispararBuscaAposDelayAsync(token);
    }

    private async Task DispararBuscaAposDelayAsync(CancellationToken token)
    {
        try
        {
            await Task.Delay(DebounceMs, token);
            if (!token.IsCancellationRequested)
                await BuscarAsync();
        }
        catch (TaskCanceledException)
        {
            // Esperado: uma tecla nova cancelou a espera anterior
        }
    }

    [RelayCommand]
    private async Task AbrirDetalhesAsync(Vinho? vinho)
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