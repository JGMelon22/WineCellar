using System.Globalization;
using WineCellar.Data;
using WineCellar.Models;

namespace WineCellar.Views;

[QueryProperty(nameof(VinhoId), "id")]
public partial class VinhoFormPage : ContentPage
{
    private readonly IVinhoRepositorio _repositorio;
    private int _vinhoId;
    private Vinho? _vinhoEmEdicao;

    public VinhoFormPage(IVinhoRepositorio repositorio)
    {
        InitializeComponent();
        _repositorio = repositorio;
        TipoPicker.ItemsSource = Enum.GetValues<TipoVinho>();
    }

    public int VinhoId
    {
        set => _vinhoId = value;
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
    }

    private async void OnSalverClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NomeEntry.Text))
        {
            await DisplayAlertAsync("Campo obrigatório", "Informe o nome do vinho.", "OK");
            return;
        }

        if (TipoPicker.SelectedItem is not TipoVinho tipoSelecionado)
        {
            await DisplayAlertAsync("Campo obrigatório", "Selecione o tipo do vinho.", "OK");
            return;
        }

        if (!int.TryParse(AnoEntry.Text, out var ano))
        {
            await DisplayAlertAsync("Valor inválido", "Informe um ano válido.", "OK");
            return;
        }

        if (!double.TryParse(NotaEntry.Text, out var nota) || nota < 0 || nota > 10)
        {
            await DisplayAlertAsync("Valor inválido", "A nota deve ser um número entre 0 e 10.", "OK");
            return;
        }

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

        if (_vinhoEmEdicao is null)
            await _repositorio.Adicionar(vinho);
        else
            await _repositorio.Atualizar(vinho);

        await Shell.Current.GoToAsync(".."); // Shorthand do Shell para voltar uma página 
    }
}