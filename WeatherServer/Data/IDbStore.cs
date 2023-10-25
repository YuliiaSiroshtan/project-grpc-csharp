namespace WeatherServer.Data;

public interface IDbStore : IDisposable
{
    void StartTransaction();
    Task<T?> SelectAsync<T>(string query, object parameters);
    Task<IEnumerable<T>> SelectListAsync<T>(string query, object parameters = null);
    Task ExecuteAsync(string query, object parameters);
    Task<T?> ExecuteAsync<T>(string query, object parameters);
    Task CommitAsync();
}