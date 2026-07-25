namespace WineCellar.Models;

public class Vinho
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    public string Regiao { get; set; } = string.Empty;
    public string Uvas { get; set; } = string.Empty;
    public TipoVinho Tipo { get; set; }
    public double Nota { get; set; }
    public bool RecomendaDecatar { get; set; }
    public string? CaminhoFoto { get; set; }
}