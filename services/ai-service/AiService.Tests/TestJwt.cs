using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AiService.Tests
{
    /// <summary>
    /// Mints JWTs identical to those issued by the auth-service, signed with the
    /// shared default test key, so [Authorize] endpoints can be exercised in isolation.
    /// </summary>
    public static class TestJwt
    {
        private const string Key = "SuperSecretKeyForLearningPlatform1234567890";

        public static string Create(int userId, string name, bool isAdmin = false)
        {
            var claims = new List<Claim>
            {
                new("id", userId.ToString()),
                new(ClaimTypes.Email, $"{name}@test.com"),
                new(ClaimTypes.Name, name)
            };
            if (isAdmin)
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            var key = Encoding.ASCII.GetBytes(Key);
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(handler.CreateToken(descriptor));
        }
    }
}
