using Microsoft.EntityFrameworkCore;
using Npgsql;
using Alumni.Data;

var builder = WebApplication.CreateBuilder(args);

// Load .env file into environment variables if it exists in content root
var envFilePath = Path.Combine(builder.Environment.ContentRootPath, ".env");
if (File.Exists(envFilePath))
{
    foreach (var line in File.ReadAllLines(envFilePath))
    {
        var trimmed = line.Trim();
        if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith('#'))
            continue;

        var parts = trimmed.Split('=', 2);
        if (parts.Length == 2)
        {
            var key = parts[0].Trim();
            var val = parts[1].Trim().Trim('"').Trim('\'');
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
            {
                Environment.SetEnvironmentVariable(key, val);
            }
        }
    }
}

// Construct database connection string securely:
// Environment variables (from .env or system) take precedence over appsettings.
var configuredConn = builder.Configuration.GetConnectionString("DefaultConnection");
var connBuilder = !string.IsNullOrWhiteSpace(configuredConn)
    ? new NpgsqlConnectionStringBuilder(configuredConn)
    : new NpgsqlConnectionStringBuilder();

var envHost = Environment.GetEnvironmentVariable("POSTGRES_HOST");
var envPort = Environment.GetEnvironmentVariable("POSTGRES_PORT");
var envDb = Environment.GetEnvironmentVariable("POSTGRES_DB");
var envUser = Environment.GetEnvironmentVariable("POSTGRES_USER");
var envPass = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");

if (!string.IsNullOrWhiteSpace(envHost))
    connBuilder.Host = envHost;
else if (string.IsNullOrWhiteSpace(connBuilder.Host))
    connBuilder.Host = "localhost";

if (!string.IsNullOrWhiteSpace(envPort) && int.TryParse(envPort, out var parsedPort))
    connBuilder.Port = parsedPort;
else if (connBuilder.Port == 0)
    connBuilder.Port = 5432;

if (!string.IsNullOrWhiteSpace(envDb))
    connBuilder.Database = envDb;
else if (string.IsNullOrWhiteSpace(connBuilder.Database))
    connBuilder.Database = "alumni_db";

if (!string.IsNullOrWhiteSpace(envUser))
    connBuilder.Username = envUser;
else if (string.IsNullOrWhiteSpace(connBuilder.Username))
    connBuilder.Username = "alumni_user";

if (!string.IsNullOrWhiteSpace(envPass))
    connBuilder.Password = envPass;

var connectionString = connBuilder.ConnectionString;

// Add services to the container.
builder.Services.AddDbContext<AlumniDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllersWithViews();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Alumni API V1");
    c.RoutePrefix = "api/swagger";
});

app.UseHttpsRedirection();
app.UseStaticFiles();

// Route root and about requests to HomeController views, and ensure /swagger opens Swagger UI
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;
    if (string.IsNullOrEmpty(path) || path == "/")
    {
        context.Request.Path = "/Home/Index";
    }
    else if (path.Equals("/about", StringComparison.OrdinalIgnoreCase))
    {
        context.Request.Path = "/Home/About";
    }
    else if (path.Equals("/swagger", StringComparison.OrdinalIgnoreCase) || path.Equals("/swagger/", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.Redirect("/api/swagger");
        return;
    }
    await next();
});

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.Run();
