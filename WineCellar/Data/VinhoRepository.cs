using SQLite;
using WineCellar.Models;

namespace WineCellar.Data;

public class VinhoRepository : IVinhoRepositorio
{
    private readonly SQLiteAsyncConnection _conexao;
    private bool _inicializado;

    public VinhoRepository()
    {
        var caminhoDb = Path.Combine(FileSystem.AppDataDirectory, "winecellar.db3");
        _conexao = new SQLiteAsyncConnection(caminhoDb);
    }

    private async Task InicializarAsync()
    {
        if (_inicializado)
            return;

        await _conexao.CreateTableAsync<Vinho>();

        _inicializado = true;
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
}