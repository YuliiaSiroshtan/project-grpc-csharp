namespace ApiClient.Models;

public class WeatherApiRequest
{
    public DateOnly Date { get; set; }
    public string? Description { get; set; }
    public int Temperature { get; set; }
}
