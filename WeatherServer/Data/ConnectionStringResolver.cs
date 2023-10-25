namespace WeatherServer.Data;

public class ConnectionStringResolver : IConnectionStringResolver
{
    private static string ConnectionStringKey => "ConnectionStrings";
    public string? Resolve => Environment.GetEnvironmentVariable(ConnectionStringKey);
}