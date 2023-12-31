using ApiClient.Models;
using ApiClient.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiClient.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public WeatherController(
        IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
        return Ok(await _weatherService.GetAllAsync());
    }

    [HttpPost]
    public async Task<ActionResult> AddWeatherAsync([FromBody] WeatherApiRequest request)
    {
        return Ok(await _weatherService.AddAsync(request));
    }

    [HttpPut]
    public async Task<ActionResult> UpdatedWeatherAsync([FromBody] WeatherApiEntity request)
    {
        return Ok(await _weatherService.UpdateAsync(request));
    }

    [HttpDelete("{entityId}")]
    public async Task<ActionResult> Delete([FromRoute] int entityId)
    {
        return Ok(await _weatherService.DeleteAsync(entityId));
    }
}