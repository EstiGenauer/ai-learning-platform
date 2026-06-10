using Microsoft.EntityFrameworkCore;
using LearningPlatformApi.Data;
using LearningPlatformApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- 1. הגדרות בסיס נתונים ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 2. הגדרות JWT ---
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

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

// --- 3. הגדרת שירות ה-AI ---
var apiKey = builder.Configuration["OpenAi:ApiKey"];
if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("⚠️ אזהרה: לא נמצא API Key ב-Configuration. שירות ה-AI לא יעבוד.");
}
else
{
    builder.Services.AddSingleton(new AiService(apiKey));
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 4. הגדרות Middleware ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// חשוב: Authentication חייב להגיע לפני Authorization!
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// --- 5. בדיקת חיבור ל-DB ---
app.MapGet("/test-db", async (AppDbContext db) =>
{
    try 
    {
        var tableCount = await db.PromptHistories.CountAsync();
        return Results.Ok($"Connected successfully! Found {tableCount} items in the history.");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Database connection failed: {ex.Message}");
    }
});

app.Run();