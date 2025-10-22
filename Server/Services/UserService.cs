using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Server.Services
{
    public class UserService
    {
        private readonly AppDbContext _db;
        private readonly string _jwtSecret;

        public UserService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _jwtSecret = config["JwtSettings:Secret"] ?? throw new Exception("JWT secret missing");
        }

        /// <summary>
        /// Login a user with username and password.
        /// Returns JWT token if successful, null otherwise.
        /// </summary>
        public string? Login(string username, string password)
        {
            User? user = _db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null || !VerifyPassword(password, user.PasswordHash))
                return null;

            return GenerateJwtToken(user);
        }

        //---------------------- Helpers ----------------------
        /// <summary>
        /// Generate JWT token for a given user
        /// </summary>
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSecret)),
                    SecurityAlgorithms.HmacSha256Signature),
                Expires = DateTime.UtcNow.AddHours(1)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Verify password (placeholder)
        /// TODO: Replace with proper hashing, e.g., BCrypt
        /// </summary>
        private bool VerifyPassword(string password, string passwordHash)
        {
            return password == passwordHash;
        }
    }
}
