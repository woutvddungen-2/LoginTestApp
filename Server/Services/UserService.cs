using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Server.Data;
using Server.Models;
using Shared.Models;

namespace Server.Services
{
    public class UserService
    {
        private readonly AppDbContext db;
        private readonly string jwtSecret;

        public UserService(AppDbContext db, IConfiguration config)
        {
            this.db = db;
            jwtSecret = config["JwtSettings:Secret"] ?? throw new Exception("JWT secret missing");
        }

        /// <summary>
        /// Register a new user with username and password.
        /// </summary>
        public async Task<UserDto> Register(string username, string password)
        {
            if (db.Users.Any(u => u.Username == username))
                throw new Exception("Username already exists");

            User user = new User{Username = username, PasswordHash = HashPassword(password)};
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username
            };
        }

        /// <summary>
        /// Login a user with username and password.
        /// Returns JWT token if successful, null otherwise.
        /// </summary>
        public string? Login(string username, string password)
        {
            User? user = db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null || !VerifyPassword(password, user.PasswordHash))
                return null;

            return GenerateJwtToken(user);
        }


        /// <summary>
        /// Retrieves user information based on the specified user ID.
        /// </summary>
        public async Task<UserDto> GetUserInfo(int Id)
        {
            User? user = await db.Users.FindAsync(Id);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            return new UserDto {Id = user.Id, Username = user.Username };
        }

        //---------------------- Helpers ----------------------
        /// <summary>
        /// Generate JWT token for a given user
        /// </summary>
        private string GenerateJwtToken(User user)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            Claim[] claims =
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            ];

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
                    SecurityAlgorithms.HmacSha256Signature),
                Expires = DateTime.UtcNow.AddHours(1)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        //----------------------- Password Hashing Helpers ----------------------
        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);

            byte[] hashBytes = new byte[48];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);

            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);

            for (int i = 0; i < 32; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                    return false;
            }

            return true;
        }
    }
}
