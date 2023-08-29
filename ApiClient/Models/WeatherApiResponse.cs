namespace ApiClient.Models;

public class WeatherApiResponse
{
    public string? Errors { get; set; }
    public List<WeatherApiEntity>? Entities { get; set; }
}
