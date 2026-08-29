using System.Globalization;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WineCellar.Data;
using WineCellar.Models;
using WineCellar.Services;
using WineCellar.Views;

namespace WineCellar.ViewModels;

[QueryProperty(nameof(VinhoId), "id")]
public partial class VinhoDetailViewModel(IVinhoRepositorio repositorio, IFotoService fotoService) : ObservableObject
{
    [ObservableProperty] private string _ano = string.Empty;

    [ObservableProperty] private string _decantarTexto = string.Empty;

    [ObservableProperty] private string _descricao = string.Empty;

    [ObservableProperty] private ImageSource? _fotoOrigem;

    [ObservableProperty] private string _nome = string.Empty;

    [ObservableProperty] private string _nota = string.Empty;

    [ObservableProperty] private string _pais = string.Empty;

    [ObservableProperty] private string _regiao = string.Empty;

    [ObservableProperty] private bool _temFoto;

    [ObservableProperty] private string _tipo = string.Empty;

    [ObservableProperty] private string _uvas = string.Empty;

    private Vinho? _vinho;

    [ObservableProperty] private int _vinhoId;

    public bool SemFoto => !TemFoto;

    // SemFoto não é [ObservableProperty] (é calculada), então precisa
    // disparar manualmente o PropertyChanged quando TemFoto mudar.
    partial void OnTemFotoChanged(bool value)
    {
        OnPropertyChanged(nameof(SemFoto));
    }

    [RelayCommand]
    private async Task CarregarAsync()
    {
        _vinho = await repositorio.ObterPorId(VinhoId);
        if (_vinho is null)
            return;

        Nome = _vinho.Nome;
        Tipo = _vinho.Tipo.ToString();
        Pais = _vinho.Pais;
        Regiao = _vinho.Regiao;
        Ano = _vinho.Ano.ToString();
        Nota = _vinho.Nota.ToString(CultureInfo.InvariantCulture);
        Uvas = _vinho.Uvas;
        Descricao = string.IsNullOrWhiteSpace(_vinho.Descricao)
            ? "Sem descrição."
            : _vinho.Descricao;
        DecantarTexto = _vinho.RecomendaDecantar
            ? "🍷 Recomenda-se decantar antes de servir."
            : "Não precisa decantar.";

        TemFoto = !string.IsNullOrWhiteSpace(_vinho.CaminhoFoto) && File.Exists(_vinho.CaminhoFoto);
        FotoOrigem = TemFoto ? ImageSource.FromFile(_vinho.CaminhoFoto) : null;
    }

    [RelayCommand]
    private async Task EditarAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(VinhoFormPage)}?id={VinhoId}");
    }

    [RelayCommand]
    private async Task ExcluirAsync()
    {
        if (_vinho is null)
            return;

        var confirmar = await Shell.Current.DisplayAlertAsync(
            "Excluir vinho",
            $"Tem certeza que deseja excluir \"{_vinho.Nome}\"?",
            "Excluir",
            "Cancelar");

        if (!confirmar)
            return;

        await repositorio.Excluir(_vinho);
        fotoService.ExcluirFoto(_vinho.CaminhoFoto);

        await Toast.Make($"\"{_vinho.Nome}\" excluído").Show();
        
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task CompartilharAsync()
    {
        if (_vinho is null)
            return;

        var texto = MontarTextoCompartilhamento(_vinho);

        var temFoto = !string.IsNullOrWhiteSpace(_vinho.CaminhoFoto)
                      && File.Exists(_vinho.CaminhoFoto);

        if (temFoto)
        {
            // Por limitação do ShareMultipleFilesRequest, precisa por em disco o texto 
            var caminhoTexto = Path.Combine(FileSystem.CacheDirectory, $"ficha_tecnica_{_vinho.Nome}.txt");
            await File.WriteAllTextAsync(caminhoTexto, texto);

            await Share.Default.RequestAsync(new ShareMultipleFilesRequest
            {
                Title = "Compartilhar vinho",
                Files =
                [
                    new ShareFile(_vinho.CaminhoFoto!),
                    new ShareFile(caminhoTexto)
                ]
            });
        }

        else
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Title = "Compartilhar vinho",
                Text = texto
            });
        }
    }

    private static string MontarTextoCompartilhamento(Vinho vinho)
    {
        List<string> linhas =
        [
            $"🍷 {vinho.Nome} ({vinho.Ano})",
            $"Tipo: {vinho.Tipo}",
            $"País: {vinho.Pais}",
            $"Uvas: {vinho.Uvas}",
            $"Nota: {vinho.Nota.ToString("0.#", CultureInfo.InstalledUICulture)}/10",
            $"Recomenda decantar: {(vinho.RecomendaDecantar ? "Sim" : "Não")}",
            "",
            vinho.Descricao
        ];

        return string.Join(Environment.NewLine, linhas);
    }
}