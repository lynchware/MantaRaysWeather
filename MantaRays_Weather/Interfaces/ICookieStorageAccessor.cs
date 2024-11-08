namespace MantaRays_Weather.Interfaces
{
    public interface ICookieStorageAccessor
    {
        ValueTask DisposeAsync();
        Task<T> GetValueAsync<T>(string key);
        Task SetValueAsync<T>(string key, T value);
    }
}