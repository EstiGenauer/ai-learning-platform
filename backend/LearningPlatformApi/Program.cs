using Microsoft.EntityFrameworkCore;
using LearningPlatformApi.Data;
using LearningPlatformApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (builder.Environment.IsEnvironment("Testing"))
    {
        var testDbName = builder.Configuration["Testing:DatabaseName"] ?? "LearningPlatformTests";
        options.UseInMemoryDatabase(testDbName);
    }
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

var apiKey = builder.Configuration["OpenAi:ApiKey"];
var openAiModel = builder.Configuration["OpenAi:Model"] ?? "gpt-4o-mini";
builder.Services.AddSingleton<IAiService>(new AiService(apiKey ?? "dummy-key", openAiModel));

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

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/test-db", async (AppDbContext db) =>
{
    try
    {
        var canConnect = await db.Database.CanConnectAsync();
        return canConnect ? Results.Ok("Connected successfully!") : Results.Problem("Cannot connect to DB.");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Database error: {ex.Message}");
    }
});

app.MapControllers();

if (!app.Environment.IsEnvironment("Testing"))
{
    // Start API immediately; migrate DB in background (avoids healthcheck/startup deadlock)
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
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordService = scope.ServiceProvider.GetRequiredService<PasswordService>();

    for (var attempt = 1; attempt <= 15; attempt++)
    {
        try
        {
            await dbContext.Database.MigrateAsync();
            await DbSeeder.SeedAsync(dbContext, passwordService);
            Console.WriteLine("Database migrated and seeded successfully.");
            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database init attempt {attempt}/15 failed: {ex.Message}");
            if (attempt == 15) throw;
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }
}

public partial class Program { }
