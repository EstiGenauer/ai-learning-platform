using Microsoft.EntityFrameworkCore;
using LearningPlatformApi.Data;
using LearningPlatformApi.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. הזרקת ה-DbContext עם PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. קריאת המפתח והוספת הגנה כדי שהשרת לא יקרוס
var apiKey = builder.Configuration["OpenAi:ApiKey"];

if (string.IsNullOrWhiteSpace(apiKey))
{
    // מדפיס אזהרה בטרמינל במקום להפיל את האפליקציה
    Console.WriteLine("⚠️ אזהרה: לא נמצא API Key ב-Configuration. השרת יעלה, אך שירות ה-AI לא יעבוד.");
}
else
{
    builder.Services.AddSingleton(new AiService(apiKey));
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ה-Build חייב להגיע אחרי כל רישומי ה-Services!
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// במקום מה שיש לך עכשיו, תוסיפי async ו-await
app.MapGet("/test-db", async (AppDbContext db) =>
{
    var tableCount = await db.PromptHistories.CountAsync(); // שימוש ב-await עם פעולה אסינכרונית
    return Results.Ok($"Connected successfully! Found {tableCount} items in the history.");
});

app.Run();

