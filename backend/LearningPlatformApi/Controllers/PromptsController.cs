using Microsoft.AspNetCore.Mvc;
using LearningPlatformApi.Services;
using LearningPlatformApi.Data;   // הוספתי בשביל ה-AppDbContext
using LearningPlatformApi.Models; // הוספתי בשביל ה-PromptHistory
using Microsoft.EntityFrameworkCore; // הוספתי בשביל ה-ToListAsync() 
using System.Linq;
namespace LearningPlatformApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromptsController : ControllerBase
    {
        private readonly AiService _aiService;
        private readonly ILogger<PromptsController> _logger;
        private readonly AppDbContext _context; // הוספתי את משתנה ה-DB

        // הוספתי את ה-AppDbContext לפרמטרים של ה-Constructor
        public PromptsController(AiService aiService, ILogger<PromptsController> logger, AppDbContext context)
        {
            _aiService = aiService;
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<string>> AskAi([FromBody] string question)
        {
            try
            {
                _logger.LogInformation("נשלחה בקשה חדשה ל-AI: {Question}", question);
                
                var response = await _aiService.GetAiResponse(question);

                // שמירה לבסיס הנתונים
                var history = new PromptHistory { Question = question, Answer = response };
                _context.PromptHistories.Add(history);
                await _context.SaveChangesAsync();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה בתהליך ה-AI או בשמירה ל-DB!");
                return StatusCode(500, "קרתה שגיאה בשרת: " + ex.Message);
            }
        }

        [HttpGet("history")]
        public ActionResult<List<PromptHistory>> GetHistory()
        {
             // מחקנו את ה-await ואת ה-async כי אנחנו משתמשים ב-ToList() רגיל
                return _context.PromptHistories
                   .OrderByDescending(h => h.CreatedAt)
                   .ToList();
        }
    }
}