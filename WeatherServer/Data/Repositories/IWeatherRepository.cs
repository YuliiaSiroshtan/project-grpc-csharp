using WeatherServer.Protos;

namespace WeatherServer.Repositories;

public interface IWeatherRepository
{
    void Add(WeatherEntity entity);
    void Delete(WeatherDeleteRequest request);
    Task<WeatherResponse> GetAll();
    void Update(WeatherEntity request);
}