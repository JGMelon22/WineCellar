namespace WineCellar.Services;

public interface IFotoService
{
    Task<string?> CapturarFotoAsync();
    Task<string?> SelecionarDaGaleriaAsync();
    void ExcluirFoto(string? caminho);
}