using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services;
using Shared.Models;
using System.Security.Claims;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductController(ProductService service)
        {
            _service = service;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetMyProducts()
        {
            int userId;
            try { userId = GetUserIdFromJwt(); }
            catch { return Unauthorized(); }

            List<ProductDto> products = await _service.GetProductsByUser(userId);
            return Ok(products.Select(p => new { p.Name, p.Price }));
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductDto dto)
        {
            int userId;
            try { userId = GetUserIdFromJwt(); }
            catch { return Unauthorized(); }

            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                UserId = userId
            };
            var created = await _service.AddProductAsync(product);
            return Ok(product);
        }

        //-------------------- HELPERS --------------------
        // Helper to extract user ID from JWT
        private int GetUserIdFromJwt()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
            if (!int.TryParse(userIdClaim.Value, out int userId))
                throw new UnauthorizedAccessException();

            return userId;
        }
    }
}
