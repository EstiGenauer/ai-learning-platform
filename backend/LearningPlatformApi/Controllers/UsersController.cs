using Microsoft.AspNetCore.Mvc;
using LearningPlatformApi.Models;

namespace LearningPlatformApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        // רשימה סטטית כדי שהנתונים יישמרו כל עוד השרת רץ
        private static List<User> _users = new List<User>
        {
            new User { Id = 1, Name = "Israel Israeli", Email = "israel@example.com" },
            new User { Id = 2, Name = "Dana Cohen", Email = "dana@example.com" }
        };

        [HttpGet]
        public IActionResult Get() => Ok(_users);

        [HttpPost]
        public IActionResult Post(User newUser)
        {
            newUser.Id = _users.Count + 1;
            _users.Add(newUser);
            return Ok(newUser);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();
            _users.Remove(user);
            return NoContent();
        }
    }
}