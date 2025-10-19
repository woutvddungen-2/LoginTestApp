namespace Server.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int UserId { get; set; }  // FK to User
        public User User { get; set; } = null!;
    }
}
