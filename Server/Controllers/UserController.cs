using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Shared.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly string jwtSecret = new JWTString().Secret; // In production, store this securely

        public UserController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // -------------------- LOGIN --------------------
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto login)
        {
            User? user = _dbContext.Users.FirstOrDefault(u => u.Username == login.Username);
            if (user == null || !VerifyPassword(login.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid username or password" });

            string? token = GenerateJwtToken(user);

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,           // ✅ Cannot be read from JavaScript
                Secure = true,             // ✅ Required for HTTPS
                SameSite = SameSiteMode.Lax, // ✅ Required for cross-site requests (WASM -> API)
            });

            return Ok(new { token });
        }

        [Authorize]
        [HttpGet("verify")]
        public IActionResult Verify()
        {
            var username = User.Identity?.Name ?? "Unknown";
            return Ok(new { loggedIn = true, username });
        }


        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return Ok(new { message = "Logged out" });
        }

        // -------------------- HELPERS --------------------
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new Claim[]
            {
            new Claim("id", user.Id.ToString()),
            new Claim("name", user.Username)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
                    SecurityAlgorithms.HmacSha256Signature),
                Expires = DateTime.UtcNow.AddHours(1)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            // TODO: Replace with real hashing (e.g., BCrypt)
            return password == passwordHash;
        }
    }
}
