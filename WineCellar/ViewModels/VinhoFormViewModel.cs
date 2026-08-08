using System.ComponentModel.DataAnnotations;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WineCellar.Data;
using WineCellar.Models;
using WineCellar.Services;
using WineCellar.Validation;

namespace WineCellar.ViewModels;

[QueryProperty(nameof(VinhoId), "id")]
public partial class VinhoFormViewModel : ObservableValidator
{
    private readonly IVinhoRepositorio _repositorio;
    private readonly IFotoService _fotoService;

    private Vinho? _vinhoEmEdicao;
    private string? _caminhoFotoOriginal;
    private string? _caminhoFotoAtual;

    [ObservableProperty] private int _vinhoId;

    [ObservableProperty] private string _titulo = "Novo Vinho";

    [ObservableProperty] [NotifyDataErrorInfo] [Required(ErrorMessage = "Informe o nome do vinho.")]
    private string _nome = string.Empty;

    [ObservableProperty] private string _descricao = string.Empty;

    [ObservableProperty] private string _pais = string.Empty;

    [ObservableProperty] private string _regiao = string.Empty;

    [ObservableProperty] private string _uvas = string.Empty;

    [ObservableProperty] [NotifyDataErrorInfo] [AnoValidation]
    private int _ano;

    [ObservableProperty] [NotifyDataErrorInfo] [Required(ErrorMessage = "Selecione o tipo de vinho")]
    private TipoVinho? _tipoSelecionado;

    [ObservableProperty] [NotifyDataErrorInfo] [NotaValidation]
    private string _notaTexto = string.Empty;

    [ObservableProperty] private bool _recomendaDecantar;

    [ObservableProperty] private ImageSource? _fotoPreview;

    [ObservableProperty] private bool _temFoto;

    public bool SemFoto => TemFoto;

    partial void OnTemFotoChanged(bool value)
    {
        OnPropertyChanged(nameof(SemFoto));
    }

    public IEnumerable<TipoVinho> TiposDisponiveis => Enum.GetValues<TipoVinho>();

    public VinhoFormViewModel(IVinhoRepositorio repositorio, IFotoService fotoService)
    {
        _repositorio = repositorio;
        _fotoService = fotoService;
    }

    [RelayCommand]
    private async Task CarregarAsync()
    {
        if (VinhoId <= 0)
            return; // Modo criação

        _vinhoEmEdicao = await _repositorio.ObterPorId(VinhoId);
        if (_vinhoEmEdicao is null)
            return;

        Titulo = "Editar Vinho";

        Nome = _vinhoEmEdicao.Nome;
        Descricao = _vinhoEmEdicao.Descricao;
        Pais = _vinhoEmEdicao.Pais;
        Regiao = _vinhoEmEdicao.Regiao;
        Uvas = _vinhoEmEdicao.Uvas;
        Ano = _vinhoEmEdicao.Ano;
        TipoSelecionado = _vinhoEmEdicao.Tipo;
        NotaTexto = _vinhoEmEdicao.Nota.ToString(CultureInfo.InvariantCulture);
        RecomendaDecantar = _vinhoEmEdicao.RecomendaDecantar;

        _caminhoFotoOriginal = _vinhoEmEdicao.CaminhoFoto;
        _caminhoFotoAtual = _caminhoFotoOriginal;
        AtualizarPreviewFoto();
    }

    [RelayCommand]
    private async Task SelecionarFotoAsync()
    {
        var opcoes = _fotoService.CameraDisponivel()
            ? new[] { "Câmera", "Galeria" }
            : new[] { "Galeria" };

        var escolha = await Shell.Current.DisplayActionSheetAsync("Fato da garrafa", "Cancelar", null, opcoes);

        string? novoCaminho = escolha switch
        {
            "Câmera" => await _fotoService.CapturarFotoAsync(),
            "Galeria" => await _fotoService.SelecionarDaGaleriaAsync(),
            _ => null
        };
        if (novoCaminho is null)
            return;

        // Se já havia uma foto escolhida NESTA sessão (diferente da original salva),
        // ela fica órfã agora — apaga pra não acumular lixo.
        if (_caminhoFotoAtual is not null && _caminhoFotoAtual != _caminhoFotoOriginal)
            _fotoService.ExcluirFoto(_caminhoFotoAtual);

        _caminhoFotoAtual = novoCaminho;
        AtualizarPreviewFoto();
    }

    [RelayCommand]
    private async Task SalvarAsync()
    {
        ValidateAllProperties();

        if (HasErrors)
        {
            var primeiroErro = GetErrors().First().ErrorMessage;
            await Shell.Current.DisplayAlertAsync("Dados inválidos", primeiroErro, "OK");
            return;
        }

        var nota = double.Parse(NotaTexto.Replace(',', '.'), CultureInfo.InvariantCulture);

        var vinho = _vinhoEmEdicao ?? new Vinho();
        vinho.Nome = Nome;
        vinho.Descricao = Descricao;
        vinho.Pais = Pais;
        vinho.Regiao = Regiao;
        vinho.Uvas = Uvas;
        vinho.Ano = Ano;
        vinho.Tipo = TipoSelecionado!.Value;
        vinho.Nota = nota;
        vinho.RecomendaDecantar = RecomendaDecantar;
        vinho.CaminhoFoto = _caminhoFotoAtual;

        if (_vinhoEmEdicao is null)
            await _repositorio.Adicionar(vinho);
        else
            await _repositorio.Atualizar(vinho);

        if (_caminhoFotoOriginal is not null && _caminhoFotoOriginal != _caminhoFotoAtual)
            _fotoService.ExcluirFoto(_caminhoFotoOriginal);

        await Shell.Current.GoToAsync("..");
    }

    private void AtualizarPreviewFoto()
    {
        TemFoto = !string.IsNullOrWhiteSpace(_caminhoFotoAtual);
        FotoPreview = TemFoto ? ImageSource.FromFile(_caminhoFotoAtual) : null;
    }
}