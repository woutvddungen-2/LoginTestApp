using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Server.Services;

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
            var token = _service.Login(login.Username, login.Password);
            if (token == null) return Unauthorized(new { message = "Invalid username or password" });

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
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
    }
}
