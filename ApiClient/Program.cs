using ApiClient.Auth;
using ApiClient.Extensions;
using ApiClient.Options;
using ApiClient.Protos;
using ApiClient.Services;
using dotenv.net;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var weatherServiceOptions = OptionsExtensions.LoadOptions<WeatherServerOptions, WeatherServerOptions.Validator>(
    builder.Configuration,
    builder.Services
);

builder.Services
    .AddGrpcClient<Weather.WeatherClient>(o =>
    {
        o.Address = new Uri(weatherServiceOptions.Url ?? throw new InvalidOperationException());
    })
    .AddCallCredentials(async (context, metadata, serviceProvider) =>
    {
        var provider = serviceProvider.GetRequiredService<ITokenProvider>();
        var token = await provider.GetTokenAsync(context.CancellationToken);
        metadata.Add(HeaderNames.Authorization, $"Bearer {token}");
    });

builder.Services.AddScoped<ITokenProvider, AppTokenProvider>();
builder.Services.AddScoped<IWeatherService, WeatherService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
