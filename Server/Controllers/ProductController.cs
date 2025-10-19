using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.Models;
using Shared.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public ProductController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetAll")]
        public IActionResult GetMyProducts()
        {
            // Extract user ID from JWT claims
            var userIdClaim = User.FindFirst("id");
            if (userIdClaim == null) return Unauthorized();
            if (!int.TryParse(userIdClaim.Value, out int userId)) return Unauthorized();

            var products = _dbContext.Products
                .Where(p => p.UserId == userId)
                .Select(p => new { p.Name, p.Price })
                .ToList();

            return Ok(products);
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddProduct([FromBody] ProductDto dto)
        {
            var userIdClaim = User.FindFirst("id");
            if (userIdClaim == null) return Unauthorized();
            if (!int.TryParse(userIdClaim.Value, out int userId)) return Unauthorized();

            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                UserId = userId
            };

            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();

            return Ok(product);
        }
    }
}
