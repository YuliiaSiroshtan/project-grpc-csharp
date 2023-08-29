using ApiClient.Models;
using Google.Protobuf.WellKnownTypes;

namespace ApiClient.Services;

public interface IWeatherService
{
    Task<Empty> AddAsync(WeatherApiRequest weatherRequest);
    Task<Empty> DeleteAsync(int entityId);
    Task<WeatherApiResponse> GetAllAsync();
    Task<Empty> UpdateAsync(WeatherApiEntity weatherRequest);
}
