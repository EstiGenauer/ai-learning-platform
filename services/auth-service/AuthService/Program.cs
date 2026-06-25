using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using Serilog;
using System.Text;
using AuthService.Data;
using AuthService.Services;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("service", "auth-service")
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddDbContext<AuthDbContext>(options =>
{
    if (builder.Environment.IsEnvironment("Testing"))
        options.UseInMemoryDatabase("AuthServiceTests");
    else
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            npgsql => npgsql.CommandTimeout(30));
});

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? "SuperSecretKey1234567890123456");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<PasswordService>();

var aiServiceUrl = builder.Configuration["Services:AiService"] ?? "http://ai-service:8080";
builder.Services.AddHttpClient<PromptStatsClient>(client =>
{
    client.BaseAddress = new Uri(aiServiceUrl);
    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:3001",
                "http://127.0.0.1:3000",
                "http://localhost:8080")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

if (!app.Environment.IsEnvironment("Testing"))
    app.UseHttpMetrics();

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "auth-service" }));

if (!app.Environment.IsEnvironment("Testing"))
    app.MapMetrics();

app.MapControllers();

if (!app.Environment.IsEnvironment("Testing"))
{
    _ = Task.Run(async () =>
    {
        try
        {
            await InitializeDatabaseAsync(app.Services);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CRITICAL: Database initialization failed: {ex.Message}");
        }
    });
}

app.Run();

static async Task InitializeDatabaseAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    var passwordService = scope.ServiceProvider.GetRequiredService<PasswordService>();

    for (var attempt = 1; attempt <= 15; attempt++)
    {
        try
        {
            await db.Database.EnsureCreatedAsync();
            await AuthSeeder.SeedAsync(db, passwordService);
            Console.WriteLine("auth-service: database ready and seeded.");
            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"auth-service DB init attempt {attempt}/15 failed: {ex.Message}");
            if (attempt == 15) throw;
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }
}

public partial class Program { }
