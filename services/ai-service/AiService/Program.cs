using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using Serilog;
using System.Text;
using AiService.Data;
using AiService.Services;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("service", "ai-service")
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddDbContext<PromptsDbContext>(options =>
{
    if (builder.Environment.IsEnvironment("Testing"))
        options.UseInMemoryDatabase("AiServiceTests");
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

var apiKey = builder.Configuration["OpenAi:ApiKey"];
var openAiModel = builder.Configuration["OpenAi:Model"] ?? "gpt-4o-mini";
var useFakeAi = string.Equals(Environment.GetEnvironmentVariable("USE_FAKE_AI"), "true", StringComparison.OrdinalIgnoreCase);

if (useFakeAi)
    builder.Services.AddSingleton<ILessonGenerator, FakeLessonGenerator>();
else
    builder.Services.AddSingleton<ILessonGenerator>(new OpenAiLessonGenerator(apiKey ?? "dummy-key", openAiModel));

var catalogServiceUrl = builder.Configuration["Services:CatalogService"] ?? "http://catalog-service:8080";
builder.Services.AddHttpClient<CatalogClient>(client =>
{
    client.BaseAddress = new Uri(catalogServiceUrl);
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

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "ai-service" }));

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
    var db = scope.ServiceProvider.GetRequiredService<PromptsDbContext>();

    for (var attempt = 1; attempt <= 15; attempt++)
    {
        try
        {
            await db.Database.EnsureCreatedAsync();
            Console.WriteLine("ai-service: database ready.");
            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ai-service DB init attempt {attempt}/15 failed: {ex.Message}");
            if (attempt == 15) throw;
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }
}

public partial class Program { }
