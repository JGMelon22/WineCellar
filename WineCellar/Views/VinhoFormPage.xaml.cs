using System.Globalization;
using WineCellar.Data;
using WineCellar.Models;
using WineCellar.Services;

namespace WineCellar.Views;

[QueryProperty(nameof(VinhoId), "id")]
public partial class VinhoFormPage : ContentPage
{
    private readonly IVinhoRepositorio _repositorio;
    private readonly IFotoService _fotoService;
    private int _vinhoId;
    private Vinho? _vinhoEmEdicao;
    private string? _caminhoFotoOriginal;
    private string? _caminhoFotoAtual;

    public int VinhoId
    {
        set => _vinhoId = value;
    }

    public VinhoFormPage(IVinhoRepositorio repositorio, IFotoService fotoService)
    {
        InitializeComponent();
        _repositorio = repositorio;
        _fotoService = fotoService;
        TipoPicker.ItemsSource = Enum.GetValues<TipoVinho>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarVinho();
    }

    private async Task CarregarVinho()
    {
        if (_vinhoId <= 0)
            return; // Modo criação

        _vinhoEmEdicao = await _repositorio.ObterPorId(_vinhoId);
        if (_vinhoEmEdicao is null)
            return;

        Title = "Editar Vinho";

        NomeEntry.Text = _vinhoEmEdicao.Nome;
        DescricaoEditor.Text = _vinhoEmEdicao.Descricao;
        PaisEntry.Text = _vinhoEmEdicao.Pais;
        RegiaoEntry.Text = _vinhoEmEdicao.Regiao;
        UvasEntry.Text = _vinhoEmEdicao.Uvas;
        AnoEntry.Text = _vinhoEmEdicao.Ano.ToString();
        NotaEntry.Text = _vinhoEmEdicao.Nota.ToString(CultureInfo.InvariantCulture);
        DecantarSwitch.IsToggled = _vinhoEmEdicao.RecomendaDecantar;
        TipoPicker.SelectedItem = _vinhoEmEdicao.Tipo;

        _caminhoFotoOriginal = _vinhoEmEdicao.CaminhoFoto;
        _caminhoFotoAtual = _caminhoFotoOriginal;
        AtualizarPreviewFoto();
    }

    private async Task<(TipoVinho tipoSelecionado, int ano, double nota)> ValidarDados()
    {
        if (string.IsNullOrWhiteSpace(NomeEntry.Text))
        {
            await DisplayAlertAsync("Campo obrigatório", "Informe o nome do vinho.", "OK");
            return (TipoVinho.Tinto, 0, 0);
        }

        if (TipoPicker.SelectedItem is not TipoVinho tipoSelecionado)
        {
            await DisplayAlertAsync("Campo obrigatório", "Selecione o tipo do vinho.", "OK");
            return (TipoVinho.Tinto, 0, 0);
        }

        if (!int.TryParse(AnoEntry.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var ano)
            || ano < 1900 || ano > DateTime.Now.Year)
        {
            await DisplayAlertAsync("Valor inválido", $"Informe um ano entre 1900 e {DateTime.Now.Year}.", "OK");
            return (tipoSelecionado, ano, 0);
        }

        var notaTexto = (NotaEntry.Text).Replace(',', '.');
        if (!double.TryParse(notaTexto, NumberStyles.Float, CultureInfo.InvariantCulture, out var nota)
            || nota < 0 || nota > 10)
        {
            await DisplayAlertAsync("Valor inválido", "A nota deve ser um número entre 0 e 10.", "OK");
        }

        return (tipoSelecionado, ano, nota);
    }

    private async void OnSelecionarFotoClicked(object? sender, EventArgs e)
    {
        var opcoes = _fotoService.CameraDisponivel()
            ? new[] { "Câmera", "Galeria" }
            : new[] { "Galeria" };

        var escolha = await DisplayActionSheetAsync("Fato da garrafa", "Cancelar", null, opcoes);

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

    private void AtualizarPreviewFoto()
    {
        if (string.IsNullOrWhiteSpace(_caminhoFotoAtual))
        {
            FotoPreview.IsVisible = false;
            return;
        }

        FotoPreview.Source = ImageSource.FromFile(_caminhoFotoAtual);
        FotoPreview.IsVisible = true;
    }

    private async void OnSalvarClicked(object? sender, EventArgs e)
    {
        var (tipoSelecionado, ano, nota) = await ValidarDados();

        var vinho = _vinhoEmEdicao ?? new Vinho();
        vinho.Nome = NomeEntry.Text;
        vinho.Descricao = DescricaoEditor.Text ?? string.Empty;
        vinho.Pais = PaisEntry.Text ?? string.Empty;
        vinho.Regiao = RegiaoEntry.Text ?? string.Empty;
        vinho.Uvas = UvasEntry.Text ?? string.Empty;
        vinho.Ano = ano;
        vinho.Tipo = tipoSelecionado;
        vinho.Nota = nota;
        vinho.RecomendaDecantar = DecantarSwitch.IsToggled;
        vinho.CaminhoFoto = _caminhoFotoAtual;

        if (_vinhoEmEdicao is null)
            await _repositorio.Adicionar(vinho);
        else
            await _repositorio.Atualizar(vinho);

        // Se a foto foi trocada (edição), a original agora está órfã — apaga do disco.
        if (_caminhoFotoOriginal is not null && _caminhoFotoOriginal != _caminhoFotoAtual)
            _fotoService.ExcluirFoto(_caminhoFotoOriginal);

        await Shell.Current.GoToAsync(".."); // Shorthand do Shell para voltar uma página 
    }
}