using WineCellar.Models;

namespace WineCellar.Data;

public class VinhoRepositorioMemoria
{
    private readonly List<Vinho> _vinhos = new()
    {
        new Vinho
        {
            Id = 1,
            Nome = "Château Margaux",
            Descricao = "Um dos grandes vinhos de Bordeaux.",
            Pais = "França",
            Regiao = "Bordeaux",
            Uvas = "Cabernet Sauvignon, Merlot",
            Ano = 2015,
            Tipo = TipoVinho.Tinto,
            Nota = 9.5,
            RecomendaDecantar = true
        },
        new Vinho
        {
            Id = 2,
            Nome = "Casa Valduga 130",
            Descricao = "Espumante brasileiro método tradicional.",
            Pais = "Brasil",
            Regiao = "Vale dos Vinhedos",
            Uvas = "Chardonnay, Pinot Noir",
            Ano = 2020,
            Tipo = TipoVinho.Espumante,
            Nota = 8.0,
            RecomendaDecantar = false
        }
    };

    public List<Vinho> ObterTodos() => _vinhos;
}