using WeatherServer.Data;
using WeatherServer.Protos;

namespace WeatherServer.Repositories;

// todo. adds dapper as orm system and integrate with SQL Server or PostgreSql db
public class WeatherRepository : IWeatherRepository
{
    private readonly IDbStore _dbStore;
    private readonly WeatherResponse _weatherResponse = new();

    public WeatherRepository(IDbStore dbStore)
    {
        _dbStore = dbStore;
    }

    public async Task<WeatherResponse> GetAll()
    {
        const string query = "SELECT id, date, description, temperature\n\tFROM public.weather;";
        var list = await _dbStore.SelectListAsync<WeatherEntity>(query, new { });
        _weatherResponse.Weathers.AddRange(list);
        return _weatherResponse;
    }

    public void Add(WeatherEntity entity)
    {
        _weatherResponse.Weathers.Add(entity);
    }

    public void Update(WeatherEntity request)
    {
        var entity = _weatherResponse.Weathers.FirstOrDefault(x => x.Id == request.Id);

        if (entity is null) return;

        entity.Temperature = request.Temperature;
        entity.Date = request.Date;
        entity.Description = request.Description;
    }

    public void Delete(WeatherDeleteRequest request)
    {
        _weatherResponse.Weathers.Remove(_weatherResponse.Weathers.FirstOrDefault(x => x.Id == request.Id));
    }
}