using System.Data;
using Dapper;
using Npgsql;
using WeatherServer.Data.DapperHandlers;

namespace WeatherServer.Data;

public class DbStore : IDbStore
{
    private readonly NpgsqlConnection _dbConnection;
    private NpgsqlTransaction _transaction = null!;

    public DbStore(
        IConnectionStringResolver connectionStringResolver
    )
    {
        SqlMapper.AddTypeHandler(JObjectHandler.Instance);

        DefaultTypeMap.MatchNamesWithUnderscores = true;

        _dbConnection = new NpgsqlConnection(connectionStringResolver.Resolve);
        _dbConnection.Open();
    }

    public void StartTransaction()
    {
        _transaction = _dbConnection.BeginTransaction();
    }

    public Task<IEnumerable<T>> SelectListAsync<T>(
        string query,
        object? parameters = null
    )
    {
        return _dbConnection.QueryAsync<T>(query, parameters);
    }

    public Task<T?> SelectAsync<T>(
        string query,
        object parameters
    )
    {
        return _dbConnection.QueryFirstOrDefaultAsync<T>(query, parameters);
    }

    public Task ExecuteAsync(
        string query,
        object parameters
    )
    {
        return _dbConnection.ExecuteAsync(query, parameters, _transaction);
    }

    public Task<T?> ExecuteAsync<T>(
        string query,
        object parameters
    )
    {
        return _dbConnection.QueryFirstOrDefaultAsync<T>(query, parameters, _transaction);
    }

    public void Dispose()
    {
        if (_transaction?.Connection is not null
            && _transaction.Connection.State != ConnectionState.Closed)
            _transaction.Connection.Close();

        if (_dbConnection?.State != ConnectionState.Closed)
            _dbConnection?.Close();

        _transaction?.Dispose();
        _dbConnection?.Dispose();
    }

    public Task CommitAsync()
    {
        return _transaction.CommitAsync();
    }
}