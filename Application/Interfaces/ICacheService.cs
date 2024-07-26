namespace Application.Interfaces;

public interface ICacheService
{
    T GetData<T>(string key);
    Task<bool> SetData<T>(string key, T value);
    object RemoveData(string key);
}