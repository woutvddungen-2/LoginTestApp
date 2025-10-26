using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Shared.Models;
using System.Security.Claims;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;

        public UserController(UserService service)
        {
            _service = service;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto login)
        {
            string? token = _service.Login(login.Username, login.Password);
            if (token == null) return Unauthorized(new { message = "Invalid username or password" });

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
            });

            return Ok(new { token });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("verify")]
        public IActionResult verify()
        {
            string username = User.Identity?.Name ?? "Unknown";
            string userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            if (!int.TryParse(userIdString, out int id))
            {
                return BadRequest("Invalid user ID");
            }
            return Ok(new UserDto { Id = id, Username = username });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetUserInfoFromDb")]
        public async Task<ActionResult<UserDto>> GetUserInfoFromDbAsync()
        {
            try
            {
                int userId = GetUserIdFromJwt();
                UserDto user = await _service.GetUserInfo(userId);
                return user;
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return Ok(new { message = "Logged out" });
        }

        private int GetUserIdFromJwt()
        {
            Claim? claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
            return int.Parse(claim.Value);
        }
    }
}
