using WineCellar.Models;

namespace WineCellar.Data;

public class VinhoRepositorioMemoria
{
    private readonly List<Vinho> _vinhos =
    [
        new()
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

        new()
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
    ];

    private int _proximoId = 3;

    public List<Vinho> ObterTodos() => _vinhos;
    public Vinho? ObterVinhoPorId(int id) => _vinhos.FirstOrDefault(v => v.Id == id);

    public void Adicionar(Vinho vinho)
    {
        vinho.Id = _proximoId;
        _vinhos.Add(vinho);
    }

    public void Atualizar(Vinho vinho)
    {
        var existente = ObterVinhoPorId(vinho.Id);
        if (existente is null)
            return;

        var indice = _vinhos.IndexOf(existente);
        _vinhos[indice] = vinho;
    }

    public void Excluir(int id)
    {
        var existente = ObterVinhoPorId(id);
        if (existente is null)
            return;

        _vinhos.Remove(existente);
    }
}