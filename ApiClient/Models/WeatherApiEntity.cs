namespace ApiClient.Models;

public class WeatherApiEntity
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public string? Description { get; set; }
    public int Temperature { get; set; }
}
