using ApiClient.Models;
using ApiClient.Protos;
using Google.Protobuf.WellKnownTypes;

namespace ApiClient.Services;

public class WeatherService : IWeatherService
{
    private readonly Weather.WeatherClient _client;


    public WeatherService(
        Weather.WeatherClient client)
    {
        _client = client;
    }

    public async Task<WeatherApiResponse> GetAllAsync()
    {
        return new WeatherApiResponse()
        {
            Entities = new List<WeatherApiEntity>
            {
                new()
                {
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    Description = "Test",
                    Temperature = 10
                },
                new()
                {
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    Description = "Test",
                    Temperature = 10
                },
            }
        };

        var response = await _client.GetAllAsync(new Empty());

        return new WeatherApiResponse
        {
            Errors = response.Errors,
            Entities = response.Weathers.Select(x => new WeatherApiEntity
            {
                Date = DateOnly.FromDateTime(x.Date.ToDateTime()),
                Description = x.Description,
                Temperature = x.Temperature
            }).ToList()
        };
    }

    public async Task<Empty> AddAsync(WeatherApiRequest weatherRequest)
    {
        var request = new WeatherRequest
        {
            Date = Timestamp.FromDateTime(weatherRequest.Date.ToDateTime(new TimeOnly(0, 0)).ToUniversalTime()),
            Description = weatherRequest.Description,
            Temperature = weatherRequest.Temperature,
        };

        return await _client.AddAsync(request);
    }

    public async Task<Empty> UpdateAsync(WeatherApiEntity weatherRequest)
    {
        var request = new WeatherEntity
        {
            Id = weatherRequest.Id,
            Date = Timestamp.FromDateTime(weatherRequest.Date.ToDateTime(new TimeOnly(0, 0)).ToUniversalTime()),
            Description = weatherRequest.Description,
            Temperature = weatherRequest.Temperature,
        };

        return await _client.UpdateAsync(request);
    }

    public async Task<Empty> DeleteAsync(int entityId)
    {
        var request = new WeatherDeleteRequest
        {
            Id = entityId
        };

        return await _client.DeleteAsync(request);
    }
}