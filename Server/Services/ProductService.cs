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
        public async Task<List<Product>> GetProductsByUser(int userId)
        {
            return await _db.Products
                .Where(p => p.UserId == userId)
                .ToListAsync();
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
