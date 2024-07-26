using System.Text.Json;
using Application.Interfaces;
using StackExchange.Redis;

namespace Infrastructure.Services;

public class CacheService : ICacheService
{
    private IDatabase _cacheDb;

    public CacheService()
    {
        var redis = ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("REDIS_CONNECTION"));
        _cacheDb = redis.GetDatabase();
    }
    
    public T GetData<T>(string key)
    {
        var value = _cacheDb.StringGet(key);
        if (!string.IsNullOrEmpty(value))
            return JsonSerializer.Deserialize<T>(value);
        
        return default;
    }

    public async Task<bool> SetData<T>(string key, T value)
    {
        if (double.TryParse(Environment.GetEnvironmentVariable("REDIS_EXPIRE_MINUTES"), out double expMinutes))
        {
            var expireTime = TimeSpan.FromMinutes(expMinutes);
            return await _cacheDb.StringSetAsync(key, JsonSerializer.Serialize(value), expireTime);
        }

        throw new InvalidOperationException("Invalid REDIS_EXPIRE_MINUTES environment variable.");
    }


    public object RemoveData(string key)
    {
        var _exist = _cacheDb.KeyExists(key);

        if (_exist)
            return _cacheDb.KeyDelete(key);
        
        return false;
    }
}