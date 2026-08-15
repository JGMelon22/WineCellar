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

    public async Task<List<Vinho>> Buscar(string? nome, TipoVinho? tipoVinho, string? pais)
    {
        await InicializarAsync();

        var query = _conexao.Table<Vinho>();

        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(v => v.Nome.Contains(nome));

        if (tipoVinho.HasValue)
            query = query.Where(v => v.Tipo == tipoVinho.Value);

        if (!string.IsNullOrWhiteSpace(pais))
            query = query.Where(v => v.Pais == pais);

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
                }
            });

        _inicializado = true;
    }
}