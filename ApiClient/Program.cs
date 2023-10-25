using ApiClient.Auth;
using ApiClient.Extensions;
using ApiClient.Options;
using ApiClient.Protos;
using ApiClient.Services;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

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
    })
    .ConfigureChannel(o => o.UnsafeUseInsecureChannelCallCredentials = true);

builder.Services.AddScoped<ITokenProvider, AppTokenProvider>();
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddCors();

IdentityModelEventSource.ShowPII = true;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:5001";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
    });
builder.Services.AddAuthorization(options =>
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "api1");
    })
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(o =>
{
    o.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin();
});

app.MapHealthChecks("/healthz");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();