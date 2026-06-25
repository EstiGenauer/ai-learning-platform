using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using AuthService.Models;
using AuthService.DTOs;
using AuthService.Services;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _context;
        private readonly IConfiguration _config;
        private readonly PasswordService _passwordService;

        public AuthController(AuthDbContext context, IConfiguration config, PasswordService passwordService)
        {
            _context = context;
            _config = config;
            _passwordService = passwordService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest(new { message = "User with this email already exists." });

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                PasswordHash = _passwordService.Hash(request.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registration successful. You can now log in." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !_passwordService.Verify(request.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid email or password." });

            var token = GenerateToken(user);
            return Ok(new AuthResponse
            {
                Token = token,
                User = new UserSummaryDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    IsAdmin = user.IsAdmin
                }
            });
        }

        private string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new("id", user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.Name)
            };

            if (user.IsAdmin)
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"] ?? "SuperSecretKeyForLearningPlatform1234567890");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}
