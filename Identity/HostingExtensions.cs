using System.Reflection;
using Duende.IdentityServer;
using Identity.Data;
using Identity.Factories;
using Identity.Models;
using Identity.Pages.Admin.ApiScopes;
using Identity.Pages.Admin.IdentityScopes;
using Identity.Pages.Portal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Serilog;

namespace Identity;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();

        builder.Services.AddCors();
        // var connectionString = builder.Configuration.GetConnectionString("Identity");

        builder.Services.AddDbContext<ApplicationDbContext>(ResolveDbContextOptions);
        // builder.Services.AddDbContext<ApplicationDbContext>(dbContextOptionsBuilder =>
        // {
        //     dbContextOptionsBuilder.UseNpgsql(
        //         connectionString,
        //         NpgsqlOptionsAction);
        // });

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services
            .AddIdentityServer()
            .AddAspNetIdentity<ApplicationUser>()
            .AddConfigurationStore(configurationStoreOptions =>
            {
                configurationStoreOptions.ResolveDbContextOptions = ResolveDbContextOptions;
            })
            .AddOperationalStore(operationalStoreOptions =>
            {
                operationalStoreOptions.ResolveDbContextOptions = ResolveDbContextOptions;
            });

        builder.Services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                // register your IdentityServer with Google at https://console.developers.google.com
                // enable the Google+ API
                // set the redirect URI to https://localhost:5001/signin-google
                options.ClientId = "copy client ID from Google here";
                options.ClientSecret = "copy client secret from Google here";
            });


        // this adds the necessary config for the simple admin/config pages
        {
            builder.Services.AddAuthorization(options =>
                options.AddPolicy("admin",
                    policy => policy.RequireClaim("sub", "1"))
            );

            builder.Services.Configure<RazorPagesOptions>(options =>
                options.Conventions.AuthorizeFolder("/Admin", "admin"));

            builder.Services.AddTransient<ClientRepository>();
            builder.Services.AddTransient<Pages.Admin.Clients.ClientRepository>();
            builder.Services.AddTransient<IdentityScopeRepository>();
            builder.Services.AddTransient<ApiScopeRepository>();
        }

        // if you want to use server-side sessions: https://blog.duendesoftware.com/posts/20220406_session_management/
        // then enable it
        //isBuilder.AddServerSideSessions();
        //
        // and put some authorization on the admin/management pages using the same policy created above
        //builder.Services.Configure<RazorPagesOptions>(options =>
        //    options.Conventions.AuthorizeFolder("/ServerSideSessions", "admin"));

        return builder.Build();
    }

    private static void NpgsqlOptionsAction(NpgsqlDbContextOptionsBuilder npgsqlDbContextOptionsBuilder)
    {
        npgsqlDbContextOptionsBuilder.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
    }

    private static void ResolveDbContextOptions(IServiceProvider serviceProvider,
        DbContextOptionsBuilder dbContextOptionsBuilder)
    {
        dbContextOptionsBuilder.UseNpgsql(
            serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("Identity"),
            NpgsqlOptionsAction);
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        app.UseCors(o =>
        {
            o.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
        });

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapRazorPages()
            .RequireAuthorization();

        return app;
    }
}