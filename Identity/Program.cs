using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using Identity;
using Identity.Data;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console(
            outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration));

    var app = builder
        .ConfigureServices()
        .ConfigurePipeline();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();

        using var scope = app.Services.CreateScope();

        await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.MigrateAsync();
        await scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.MigrateAsync();
        await scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.MigrateAsync();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        if (await userManager.FindByNameAsync("thomas.clark") == null)
            await userManager.CreateAsync(new ApplicationUser
            {
                UserName = "test@example.com",
                Email = "test@example.com",
                GivenName = "John",
                FamilyName = "Rider"
            }, "Qwerty1!");

        var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

        if (!await configurationDbContext.Clients.AnyAsync())
        {
            await configurationDbContext.Clients.AddRangeAsync(
                new Client
                {
                    ClientId = "4ecc4153-daf9-4eca-8b60-818a63637a81",
                    ClientSecrets = new List<Secret> { new("secret".Sha512()) },
                    ClientName = "Angular Application",
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowedScopes = new List<string> { "openid", "profile", "email" },
                    RedirectUris = new List<string> { "http://localhost:4200" },
                    ClientUri = "http://localhost:4200",
                    AllowOfflineAccess = true,
                    AllowedCorsOrigins = new List<string> { "http://localhost:4200" },
                    PostLogoutRedirectUris = new List<string> { "http://localhost:4200/signout-callback-oidc" }
                }.ToEntity());

            await configurationDbContext.SaveChangesAsync();
        }

        if (!await configurationDbContext.IdentityResources.AnyAsync())
        {
            await configurationDbContext.IdentityResources.AddRangeAsync(
                new IdentityResources.OpenId().ToEntity(),
                new IdentityResources.Profile().ToEntity(),
                new IdentityResources.Email().ToEntity());

            await configurationDbContext.SaveChangesAsync();
        }
    }

    app.Run();
}
catch (Exception ex) when (
    // https://github.com/dotnet/runtime/issues/60600
    ex.GetType().Name is not "StopTheHostException"
    // HostAbortedException was added in .NET 7, but since we target .NET 6 we
    // need to do it this way until we target .NET 8
    && ex.GetType().Name is not "HostAbortedException"
)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}