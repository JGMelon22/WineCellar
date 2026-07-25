using System.Globalization;
using WineCellar.Data;
using WineCellar.Models;

namespace WineCellar.Views;

[QueryProperty(nameof(VinhoId), "id")]
public partial class VinhoFormPage : ContentPage
{
    private readonly VinhoRepositorioMemoria _repositorio;
    private Vinho? _vinhoEmEdicao;

    public int VinhoId
    {
        set => CarregarVinho(value);
    }

    public VinhoFormPage(VinhoRepositorioMemoria repositorio)
    {
        InitializeComponent();
        _repositorio = repositorio;
        TipoPicker.ItemsSource = Enum.GetValues<TipoVinho>();
    }

    private void CarregarVinho(int id)
    {
        if (id <= 0)
            return; // Modo criação

        _vinhoEmEdicao = _repositorio.ObterVinhoPorId(id);
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

        Vinho vinho = _vinhoEmEdicao ?? new();
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
            _repositorio.Adicionar(vinho);
        else
            _repositorio.Atualizar(vinho);
        
        await Shell.Current.GoToAsync(".."); // Shorthand do Shell para voltar uma página 
    }
}