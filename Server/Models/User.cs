namespace Server.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;



        // Relationships
        public List<Product> Products { get; set; } = new();
        public List<ChatGroupMember> GroupMemberships { get; set; } = new();
        public List<Message> Messages { get; set; } = new();
    }
}
