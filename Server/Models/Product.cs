using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")] // 2 decimal places only
        public decimal Price { get; set; }
        public int UserId { get; set; }  // FK to User
        public User User { get; set; } = null!;
    }
}
