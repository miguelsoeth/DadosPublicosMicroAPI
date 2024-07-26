using Application.Dtos;
using MongoDB.Bson;

namespace Application.Interfaces;

public interface IMongoRepository<T> where T : class
{
    Task CreateAsync(Resposta<T> dados);
    Task<Resposta<T>> GetLastAsync(string documento);
    public Task<List<DadosHistoricoLote>> GetHistoricoLoteAsync(int pageNumber, int pageSize, string? userId);
    public Task<List<DadosHistorico>> GetHistoricoPesquisa(int pageNumber, int pageSize, string usuarioFilter,
        string cnpjFilter, string? userId);
    public Task<Resposta<T>> GetPesquisa(string id);
    Task<long> GetTotalBatchCountAsync(string? userId);
    Task<long> GetTotalCountAsync(string usuarioFilter, string cnpjFilter, string? userId);
}