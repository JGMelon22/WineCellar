namespace WineCellar.Services;

public class FotoService : IFotoService
{
    private const string PastaFotos = "Fotos";

    public bool CameraDisponivel()
    {
        return MediaPicker.Default.IsCaptureSupported;
    }

    public async Task<string?> CapturarFotoAsync()
    {
        var arquivoOrigem = await MediaPicker.Default.CapturePhotoAsync();
        return await SalvarNoAppDataAsync(arquivoOrigem);
    }

    public async Task<string?> SelecionarDaGaleriaAsync()
    {
        var arquivoOrigem = await MediaPicker.Default.PickPhotosAsync();
        return await SalvarNoAppDataAsync(arquivoOrigem.FirstOrDefault());
    }

    public void ExcluirFoto(string? caminho)
    {
        if (string.IsNullOrEmpty(caminho))
            return;

        try
        {
            if (File.Exists(caminho))
                File.Delete(caminho);
        }
        catch (IOException)
        {
            // Falha silenciosa proposital: um arquivo órfão não impede o app de funcionar.
        }
    }

    private static async Task<string?> SalvarNoAppDataAsync(FileResult? arquivoOrigem)
    {
        if (arquivoOrigem is null)
            return null;

        var pastaDestino = Path.Combine(FileSystem.AppDataDirectory, PastaFotos);
        Directory.CreateDirectory(pastaDestino);

        var extensao = Path.GetExtension(arquivoOrigem.FileName);
        var nomeArquivo = $"{Guid.NewGuid()}{extensao}";
        var caminhoDestino = Path.Combine(pastaDestino, nomeArquivo);

        await using var streamOriegem = await arquivoOrigem.OpenReadAsync();
        await using var streamDestino = File.Create(caminhoDestino);

        await streamOriegem.CopyToAsync(streamDestino);

        return caminhoDestino;
    }
}