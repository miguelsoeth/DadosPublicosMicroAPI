using Application.Dtos;

namespace Application.Interfaces;

public interface IMongoRepository<T> where T : class
{
    Task CreateAsync(Resposta<T> dados);
    Task<Resposta<T>> GetLastAsync(string documento);
}