using inzynierka_geska.HealthChecks;
using inzynierka_geska.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json.Serialization;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // CFG secret
        var userSecretsConfiguration = new ConfigurationBuilder()
            .SetBasePath(builder.Environment.ContentRootPath)
            .AddUserSecrets<Program>()
            .Build(); // Ta metoda œci¹ga connection string z secrets.json

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddDbContext<ModelContext>(); // ModelContext injection

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        });

        builder.Services.AddSingleton<IConfiguration>(userSecretsConfiguration);

        builder.Services.AddControllersWithViews();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddDistributedMemoryCache();

        var connectionString = userSecretsConfiguration["ConnectionStrings:HealthTestConnectionString"];
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException("ConnectionString not found");
        }

        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromSeconds(10);
            options.Cookie.HttpOnly = false;
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddHealthChecks()
            .AddCheck(
                "firma_rodzinna_db",
                new HealthCheck(connectionString),
                HealthStatus.Unhealthy,
                new string[] { "Database" });

        builder.Services.AddCors();

        // Konfiguracja autoryzacji i uwierzytelniania
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login"; // Œcie¿ka do logowania
                options.AccessDeniedPath = "/Account/AccessDenied"; // Œcie¿ka dostêpu zabronionego
            });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.UseAuthentication();

        app.MapGet("/", () => "Hello World!");

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller}/{action=Index}/{id?}");

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true, // Warunek na wykonanie sprawdzania zdrowia
            AllowCachingResponses = false, // Wy³¹cz cache odpowiedzi (opcjonalne)
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = 200, // Kod odpowiedzi dla zdrowego stanu
                [HealthStatus.Degraded] = 200, // Kod odpowiedzi dla stanu zdegradowanego
                [HealthStatus.Unhealthy] = 503 // Kod odpowiedzi dla stanu niezdrowego
            }
        });

        app.MapFallbackToFile("index.html");
        app.Run();
    }
}
