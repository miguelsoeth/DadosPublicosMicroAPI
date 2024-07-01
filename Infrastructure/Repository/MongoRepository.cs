using System.Net;
using Application.Dtos;
using Application.Interfaces;
using MongoDB.Driver;

namespace Infrastructure.Repository;

public class MongoRepository<T> : IMongoRepository<T> where T : class
{
    private readonly IMongoCollection<Resposta<T>> _collection;
    
    public MongoRepository(string collectionName)
    {
        var mongoClient = new MongoClient(Environment.GetEnvironmentVariable("MONGO_CONNECTION"));
        var mongoDatabase = mongoClient.GetDatabase(Environment.GetEnvironmentVariable("MONGO_DBNAME"));
        _collection = mongoDatabase.GetCollection<Resposta<T>>(collectionName);
    }
    
    public async Task CreateAsync(Resposta<T> resposta)
    {
        await _collection.InsertOneAsync(resposta);
    }

    public async Task<Resposta<T>> GetLastAsync(string documento)
    {
        return await _collection.Find(x => x.Documento == documento && x.Date >= DateTime.UtcNow.AddDays(-1))
            .SortByDescending(x => x.Date)
            .FirstOrDefaultAsync();
    }
}