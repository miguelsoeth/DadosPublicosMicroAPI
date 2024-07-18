using System.Net;
using Application.Dtos;
using Application.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
        return await _collection.Find(x => x.Date >= DateTime.UtcNow.AddDays(-1))
            .SortByDescending(x => x.Date)
            .FirstOrDefaultAsync();
    }
    
    public async Task<List<DadosHistoricoLote>> GetHistoricoLoteAsync(int pageNumber, int pageSize)
    {
        var pipeline = new[]
        {
            new BsonDocument("$match", new BsonDocument("lote", new BsonDocument("$ne", BsonNull.Value))),
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", "$lote" },
                { "profile", new BsonDocument("$first", "$perfil") },
                { "registerDate", new BsonDocument("$first", "$Date") },
                { "startDate", new BsonDocument("$first", "$dataInicio") },
                { "endDate", new BsonDocument("$last", "$dataFinal") },
                { "quantity", new BsonDocument("$first", "$quantidade") },
                { "concluded", new BsonDocument("$sum", 1) }
            }),
            new BsonDocument("$addFields", new BsonDocument("status", new BsonDocument("$cond", new BsonDocument
            {
                { "if", new BsonDocument("$lt", new BsonArray { "$concluded", "$quantity" }) },
                { "then", "Processando" },
                { "else", "Processado" }
            }))),
            new BsonDocument("$sort", new BsonDocument("registerDate", -1)),
            new BsonDocument("$skip", (pageNumber - 1) * pageSize),
            new BsonDocument("$limit", pageSize)
        };

        var aggregateOptions = new AggregateOptions { AllowDiskUse = true }; // Ensure large datasets can be processed

        var cursor = await _collection.AggregateAsync<BsonDocument>(pipeline, aggregateOptions);
        var result = await cursor.ToListAsync();

        // Assuming that DadosHistorico class has properties matching the fields in the pipeline result
        var aggregatedResults = result.Select(doc => new DadosHistoricoLote
        {
            Id = doc["_id"].AsString,
            Profile = doc["profile"].AsString,
            RegisterDate = doc["registerDate"].ToUniversalTime(),
            StartDate = doc["startDate"].ToUniversalTime(),
            EndDate = doc["endDate"].ToUniversalTime(),
            Quantity = doc["quantity"].AsInt32,
            Concluded = doc["concluded"].AsInt32,
            Status = doc["status"].AsString
        }).ToList();

        return aggregatedResults;
    }

    public async Task<List<DadosHistorico>> GetHistoricoPesquisa(int pageNumber, int pageSize, string usuarioFilter,
        string cnpjFilter, string? userId)
    {
        var usuarioRegex = usuarioFilter != null ? new BsonRegularExpression(usuarioFilter, "i") : new BsonRegularExpression("");
        var usuarioIdRegex = userId != null ? new BsonRegularExpression(userId, "i") : new BsonRegularExpression("");
        
        // Define the match stage based on provided filters
        var matchStage = new BsonDocument("$match", new BsonDocument
        {
            { "usuarioId", new BsonDocument("$regex", usuarioIdRegex ?? "") },
            { "usuario", new BsonDocument("$regex", usuarioRegex ?? "") },
            { "DadosRetorno.Data.Cnpj", new BsonDocument("$regex", cnpjFilter ?? "") }
        });
        var pipeline = new[]
        {
            matchStage,
            new BsonDocument("$project", new BsonDocument
            {
                { "_id", 1 },
                { "Date", 1 },
                { "Cnpj", "$DadosRetorno.Data.Cnpj" },
                { "RazaoSocial", "$DadosRetorno.Data.RazaoSocial" },
                { "usuario", 1 }
            }),
            new BsonDocument("$sort", new BsonDocument("Date", -1)),
            new BsonDocument("$skip", (pageNumber - 1) * pageSize),
            new BsonDocument("$limit", pageSize)
        };

        var aggregateOptions = new AggregateOptions { AllowDiskUse = true }; // Ensure large datasets can be processed

        var cursor = await _collection.AggregateAsync<BsonDocument>(pipeline, aggregateOptions);
        var result = await cursor.ToListAsync();

        // Assuming that DadosHistorico class has properties matching the fields in the pipeline result
        var aggregatedResults = result.Select(doc => new DadosHistorico
        {
            _id = doc["_id"].AsObjectId.ToString(),
            usuario = doc["usuario"].AsString,
            Date = doc["Date"].ToUniversalTime(),
            Cnpj = doc["Cnpj"].AsString,
            RazaoSocial = doc["RazaoSocial"].AsString,
        }).ToList();

        return aggregatedResults;
    }
    
    public async Task<Resposta<T>> GetPesquisa(string id)
    {
        var result = await _collection.Find(x => x._id == id).FirstOrDefaultAsync();

        return result;
    }

    public async Task<long> GetTotalBatchCountAsync()
    {
        var pipeline = new[]
        {
            BsonDocument.Parse(@"{
            $match: { lote: { $ne: null } }
        }"),
            BsonDocument.Parse(@"{
            $group: { _id: ""$lote"", count: { $sum: 1 } }
        }"),
            BsonDocument.Parse(@"{
            $count: ""count""
        }")
        };

        var aggregateCursor = await _collection.AggregateAsync<BsonDocument>(pipeline);
        var result = await aggregateCursor.FirstOrDefaultAsync();

        if (result == null || !result.Contains("count"))
        {
            return 0;
        }

        return result["count"].ToInt64();
    }
    
    public async Task<long> GetTotalCountAsync(string usuarioFilter, string cnpjFilter, string? userId)
    {
        var usuarioRegex = usuarioFilter != null ? new BsonRegularExpression(usuarioFilter, "i") : new BsonRegularExpression("");
        var usuarioIdRegex = new BsonRegularExpression(userId ?? "", "i");

        // Define the match stage based on provided filters
        var matchStage = new BsonDocument("$match", new BsonDocument
        {
            { "usuarioId", new BsonDocument("$regex", usuarioIdRegex) },
            { "usuario", new BsonDocument("$regex", usuarioRegex) },
            { "DadosRetorno.Data.Cnpj", new BsonDocument("$regex", cnpjFilter ?? "") }
        });
        
        var totalCountPipeline = new[]
        {
            matchStage,
            new BsonDocument("$count", "total")
        };
        
        var aggregateOptions = new AggregateOptions { AllowDiskUse = true };
        var totalCountCursor = await _collection.AggregateAsync<BsonDocument>(totalCountPipeline, aggregateOptions);
        var totalCountResult = await totalCountCursor.FirstOrDefaultAsync();
        var totalCount = totalCountResult != null ? totalCountResult.GetValue("total", 0).ToInt64() : 0;
        return totalCount;
    }
}