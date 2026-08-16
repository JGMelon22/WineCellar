using SQLite;
using WineCellar.Models;

namespace WineCellar.Data;

public class VinhoRepositorio : IVinhoRepositorio
{
    private readonly SQLiteAsyncConnection _conexao;
    private bool _inicializado;

    public VinhoRepositorio()
    {
        var caminhoDb = Path.Combine(FileSystem.AppDataDirectory, "winecellar.db3");
        _conexao = new SQLiteAsyncConnection(caminhoDb);
    }

    public async Task<List<Vinho>> ObterTodos()
    {
        await InicializarAsync();
        return await _conexao.Table<Vinho>().ToListAsync();
    }

    public async Task<Vinho?> ObterPorId(int id)
    {
        await InicializarAsync();
        return await _conexao.Table<Vinho>()
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task Adicionar(Vinho vinho)
    {
        await InicializarAsync();
        await _conexao.InsertAsync(vinho);
    }

    public async Task Atualizar(Vinho vinho)
    {
        await InicializarAsync();
        await _conexao.UpdateAsync(vinho);
    }

    public async Task Excluir(Vinho vinho)
    {
        await InicializarAsync();
        await _conexao.DeleteAsync(vinho);
    }

    public async Task<List<Vinho>> Buscar(
        string? nome,
        TipoVinho? tipoVinho,
        string? pais,
        CampoOrdenacao campoOrdenacao,
        bool ordemCrescente
    )
    {
        await InicializarAsync();

        var query = _conexao.Table<Vinho>();

        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(v => v.Nome.Contains(nome));

        if (tipoVinho.HasValue)
            query = query.Where(v => v.Tipo == tipoVinho.Value);

        if (!string.IsNullOrWhiteSpace(pais))
            query = query.Where(v => v.Pais == pais);

        query = (campoOrdenacao, ordemCrescente) switch
        {
            (CampoOrdenacao.Nome, true) => query.OrderBy(v => v.Nome),
            (CampoOrdenacao.Nome, false) => query.OrderByDescending(v => v.Nome),
            (CampoOrdenacao.Ano, true) => query.OrderBy(v => v.Ano),
            (CampoOrdenacao.Ano, false) => query.OrderByDescending(v => v.Ano),
            (CampoOrdenacao.Nota, true) => query.OrderBy(v => v.Nota),
            (CampoOrdenacao.Nota, false) => query.OrderByDescending(v => v.Nota),
            _ => query
        };

        return await query.ToListAsync();
    }

    public async Task<List<string>> ObterPaisesDistintos()
    {
        await InicializarAsync();

        var vinhos = await _conexao.Table<Vinho>().ToListAsync();

        return vinhos
            .Select(v => v.Pais)
            .Distinct()
            .OrderBy(p => p)
            .ToList();
    }

    private async Task InicializarAsync()
    {
        if (_inicializado)
            return;

        await _conexao.CreateTableAsync<Vinho>();

        var quantidade = await _conexao.Table<Vinho>().CountAsync();
        if (quantidade == 0)
            await _conexao.InsertAllAsync(new[]
            {
                new Vinho
                {
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
                    Nome = "Casa Valduga 130",
                    Descricao = "Espumante brasileiro método tradicional.",
                    Pais = "Brasil",
                    Regiao = "Vale dos Vinhedos",
                    Uvas = "Chardonnay, Pinot Noir",
                    Ano = 2020,
                    Tipo = TipoVinho.Espumante,
                    Nota = 8.0,
                    RecomendaDecantar = false
                },
                new Vinho
                {
                    Nome = "Catena Zapata Malbec",
                    Descricao = "Malbec argentino encorpado, com frutas negras e toques de baunilha.",
                    Pais = "Argentina",
                    Regiao = "Mendoza",
                    Uvas = "Malbec",
                    Ano = 2019,
                    Tipo = TipoVinho.Tinto,
                    Nota = 9.2,
                    RecomendaDecantar = true
                },
                new Vinho
                {
                    Nome = "Cloudy Bay Sauvignon Blanc",
                    Descricao = "Branco neozelandês vibrante, com notas de maracujá e ervas frescas.",
                    Pais = "Nova Zelândia",
                    Regiao = "Marlborough",
                    Uvas = "Sauvignon Blanc",
                    Ano = 2022,
                    Tipo = TipoVinho.Branco,
                    Nota = 8.9,
                    RecomendaDecantar = false
                },
                new Vinho
                {
                    Nome = "Vega Sicilia Único",
                    Descricao = "Grande tinto espanhol, complexo e elegante, com longo envelhecimento.",
                    Pais = "Espanha",
                    Regiao = "Ribera del Duero",
                    Uvas = "Tempranillo, Cabernet Sauvignon",
                    Ano = 2014,
                    Tipo = TipoVinho.Tinto,
                    Nota = 9.7,
                    RecomendaDecantar = true
                },
                new Vinho
                {
                    Nome = "Miolo Millésime",
                    Descricao = "Espumante brasileiro brut, com perlage fina e aromas cítricos.",
                    Pais = "Brasil",
                    Regiao = "Vale dos Vinhedos",
                    Uvas = "Chardonnay, Pinot Noir",
                    Ano = 2021,
                    Tipo = TipoVinho.Espumante,
                    Nota = 8.5,
                    RecomendaDecantar = false
                },
                new Vinho
                {
                    Nome = "Château d'Yquem",
                    Descricao = "Sauternes francês lendário, doce e complexo, com notas de mel e damasco.",
                    Pais = "França",
                    Regiao = "Sauternes, Bordeaux",
                    Uvas = "Sémillon, Sauvignon Blanc",
                    Ano = 2016,
                    Tipo = TipoVinho.Rose,
                    Nota = 9.9,
                    RecomendaDecantar = false
                },
                new Vinho
                {
                    Nome = "Errázuriz Don Maximiano",
                    Descricao = "Tinto chileno icono, com cassis, especiarias e taninos refinados.",
                    Pais = "Chile",
                    Regiao = "Aconcagua Valley",
                    Uvas = "Cabernet Sauvignon, Syrah, Carménère",
                    Ano = 2018,
                    Tipo = TipoVinho.Tinto,
                    Nota = 9.1,
                    RecomendaDecantar = true
                },
                new Vinho
                {
                    Nome = "Casal Garcia Vinho Verde",
                    Descricao = "Branco português leve, fresco e levemente efervescente, ideal para verão.",
                    Pais = "Portugal",
                    Regiao = "Vinho Verde",
                    Uvas = "Loureiro, Arinto",
                    Ano = 2023,
                    Tipo = TipoVinho.Branco,
                    Nota = 7.8,
                    RecomendaDecantar = false
                },
                new Vinho
                {
                    Nome = "Pizzato Alma Unica Merlot",
                    Descricao = "Merlot brasileiro com frutas vermelhas maduras e toques de chocolate.",
                    Pais = "Brasil",
                    Regiao = "Vale dos Vinhedos",
                    Uvas = "Merlot",
                    Ano = 2020,
                    Tipo = TipoVinho.Tinto,
                    Nota = 8.3,
                    RecomendaDecantar = true
                }
            });

        _inicializado = true;
    }
}