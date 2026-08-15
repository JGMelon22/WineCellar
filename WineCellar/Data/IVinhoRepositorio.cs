using WineCellar.Models;

namespace WineCellar.Data;

public interface IVinhoRepositorio
{
    Task<List<Vinho>> ObterTodos();
    Task<Vinho?> ObterPorId(int id);
    Task Adicionar(Vinho vinho);
    Task Atualizar(Vinho vinho);
    Task Excluir(Vinho vinho);
    Task<List<Vinho>> Buscar(string? nome, TipoVinho? tipoVinho, string? pais);
    Task<List<string>> ObterPaisesDistintos();
}