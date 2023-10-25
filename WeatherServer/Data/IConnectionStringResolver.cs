namespace WeatherServer.Data;

public interface IConnectionStringResolver
{
    string? Resolve { get; }
}