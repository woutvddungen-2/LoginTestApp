using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using Shared.Models;

namespace Server.Services
{
    public class ProductService
    {
        private readonly AppDbContext _db;

        public ProductService(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Get all products from the database, ordered by name.
        /// </summary>
        public async Task<List<ProductDto>> GetProductsByUser(int userId)
        {
            
            List<Product> pruducts = await _db.Products
                .Where(p => p.UserId == userId)
                .ToListAsync();
            List<ProductDto> productsDto = new List<ProductDto>();
            foreach (Product p in pruducts)
            {
                productsDto.Add(new ProductDto { Name = p.Name, Price = p.Price});
            }
            return productsDto;
        }


        /// <summary>
        /// Add a new product to the database.
        /// </summary>
        public async Task<Product> AddProductAsync(Product product)
        {
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return product;
        }
    }
}
