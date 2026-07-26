using SQLite;

namespace WineCellar.Models;

public class Vinho
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    public string Regiao { get; set; } = string.Empty;
    public string Uvas { get; set; } = string.Empty;
    public int Ano { get; set; }
    public TipoVinho Tipo { get; set; }
    public double Nota { get; set; }
    public bool RecomendaDecantar { get; set; }
    public string? CaminhoFoto { get; set; }
}