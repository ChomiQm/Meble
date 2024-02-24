using Meble.Server.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using Meble.Server.Models;
using HealthChecks.UI.Client;
using Meble.Server.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new ArgumentNullException("ConnectionString for 'DefaultConnection' not found");
}


builder.Services.AddDbContext<ModelContext>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.BearerScheme;
    options.DefaultChallengeScheme = IdentityConstants.BearerScheme;

})
    .AddBearerToken(IdentityConstants.BearerScheme)
    .AddIdentityCookies();

builder.Services.AddAuthorizationBuilder();

builder.Services.AddIdentityCore<User>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ModelContext>()
    .AddApiEndpoints();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(5);
    options.LoginPath = "/login";
    options.SlidingExpiration = true;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
});

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<Meble.Server.Services.AzureBlobService>();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "OAuth2 Authorization header",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

});

var healthTestConnectionString = builder.Configuration.GetConnectionString("HealthTestConnectionString");
if (string.IsNullOrEmpty(healthTestConnectionString))
{
    throw new ArgumentNullException("ConnectionString not found");
}

builder.Services.AddHealthChecks()
    .AddCheck<HealthCheck>("database");


builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", builder =>
    {
        builder.WithOrigins("https://localhost:5173")
                .AllowAnyMethod()
                .WithHeaders("content-type", "authorization") // Dodaj "authorization" tutaj
                .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    Predicate = _ => true,
    AllowCachingResponses = false,
    ResultStatusCodes =
{
[HealthStatus.Healthy] = 200,
[HealthStatus.Degraded] = 200,
[HealthStatus.Unhealthy] = 503
}
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("DefaultCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();
app.MapIdentityApi<User>();
app.MapGet("/", (ClaimsPrincipal user) => $"{user.Identity!.Name}")
    .RequireAuthorization();

app.MapControllers();

//seed initial data
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Admin", "Manager", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    string email = "chomiczek667@o2.pl";
    string password = "Pysiec123!";

    if (await userManager.FindByNameAsync(email) == null)
    {
        var user = new User
        {
            Id = new Guid().ToString(),
            Email = email,
            UserName = email,
            UserDatas = null
        };
        await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, "Admin");
    }

}

app.Run();