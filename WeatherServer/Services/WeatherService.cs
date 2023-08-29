using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using WeatherServer.Protos;
using WeatherServer.Repositories;

namespace WeatherServer.Services;

[Authorize]
public class WeatherService : Weather.WeatherBase
{
    private readonly IWeatherRepository _weatherRepository;

    public WeatherService(IWeatherRepository weatherRepository)
    {
        _weatherRepository = weatherRepository;
    }

    public override Task<Empty> Add(WeatherRequest request, ServerCallContext context)
    {
        _weatherRepository.Add(new WeatherEntity
        {
            Date = request.Date,
            Description = request.Description,
            Temperature = request.Temperature
        });

        return Task.FromResult(new Empty());
    }

    public override Task<WeatherResponse> GetAll(Empty request, ServerCallContext context)
    {
        return Task.FromResult(_weatherRepository.GetAll());
    }

    public override Task<Empty> Update(WeatherEntity request, ServerCallContext context)
    {
        _weatherRepository.Update(request);
        return Task.FromResult(new Empty());
    }

    public override Task<Empty> Delete(WeatherDeleteRequest request, ServerCallContext context)
    {
        _weatherRepository.Delete(request);
        return Task.FromResult(new Empty());
    }
}
