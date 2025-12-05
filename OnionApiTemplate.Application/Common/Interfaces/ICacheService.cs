namespace Khazen.Application.Common.Interfaces
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

        Task RemoveAsync(string key);
        Task RemoveByPatternAsync(string pattern);
    }
}
