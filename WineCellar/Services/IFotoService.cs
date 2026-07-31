namespace WineCellar.Services;

public interface IFotoService
{
    bool CameraDisponivel();
    Task<string?> CapturarFotoAsync();
    Task<string?> SelecionarDaGaleriaAsync();
    void ExcluirFoto(string? caminho);
}