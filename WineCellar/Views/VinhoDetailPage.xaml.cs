using System.Globalization;
using WineCellar.Data;
using WineCellar.Models;

namespace WineCellar.Views;

[QueryProperty(nameof(VinhoId), "id")]
public partial class VinhoDetailPage : ContentPage
{
    private readonly VinhoRepositorioMemoria _repositorio;
    private Vinho? _vinho;
    private int _vinhoId;

    public VinhoDetailPage(VinhoRepositorioMemoria repositorio)
    {
        InitializeComponent();
        _repositorio = repositorio;
    }

    public int VinhoId
    {
        set
        {
            _vinhoId = value;
            CarregarVinho();
        }
    }

    private void CarregarVinho()
    {
        _vinho = _repositorio.ObterVinhoPorId(_vinhoId);
        if (_vinho is null)
            return;

        NomeLabel.Text = _vinho.Nome;
        TipoLabel.Text = _vinho.Tipo.ToString();
        PaisLabel.Text = _vinho.Pais;
        RegiaoLabel.Text = _vinho.Regiao;
        AnoLabel.Text = _vinho.Ano.ToString();
        NotaLabel.Text = _vinho.Nota.ToString(CultureInfo.InvariantCulture);
        UvasLabel.Text = _vinho.Uvas;
        DescricaoLabel.Text = string.IsNullOrWhiteSpace(_vinho.Descricao)
            ? "Sem descrição."
            : _vinho.Descricao;
        DecantarLabel.Text = _vinho.RecomendaDecantar
            ? "🍷 Recomenda-se decantar antes de servir."
            : "Não precisa decantar.";
    }

    private async void OnEditarClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(VinhoFormPage)}?id={_vinhoId}");
    }

    private async void OnExcluirClicked(object? sender, EventArgs e)
    {
        if (_vinho is null)
            return;

        var confirmar = await DisplayAlertAsync(
            "Excluir vinho",
            $"Tem certeza que deseja excluir \"{_vinho.Nome}\"?",
            "Excluir",
            "Cancelar");

        if (!confirmar)
            return;

        _repositorio.Excluir(_vinhoId);
        await Shell.Current.GoToAsync("..");
    }
}