using Application.Dtos;
using MongoDB.Bson;

namespace Application.Interfaces;

public interface IMongoRepository<T> where T : class
{
    Task CreateAsync(Resposta<T> dados);
    Task<Resposta<T>> GetLastAsync(string documento);
    public Task<List<DadosHistoricoLote>> GetAggregatedResultsAsync(int pageNumber, int pageSize);
    Task<long> GetTotalBatchCountAsync();
    Task<long> GetTotalCountAsync();
}