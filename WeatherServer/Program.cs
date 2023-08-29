using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WeatherServer.Repositories;
using WeatherServer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddSingleton<IWeatherRepository, WeatherRepository>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x =>
{
    var key = Encoding.UTF8.GetBytes("So either use another (symmetric) signature algorithm (and ensure that your key size is valid for this algorithm)");
    x.RequireHttpsMetadata = false;

    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = "test",
        ValidateIssuer = true,

        ValidAudience = "test",
        ValidateAudience = true,

        RequireExpirationTime = true,
        ValidateLifetime = true,

        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String("gTHIcLUuAMu/j5JHaN4WjRD7LABfDtT4iYCFzGJ6b2dHCkx9o4WOlPH8PP3vnKZWQ1YhoDF4A/GcuqTfK0hor/2jQyrtP2RM55bciN8/fONfBJBuvsuV8N1gxX+3mxbS")),
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<WeatherService>();

app.Run();
