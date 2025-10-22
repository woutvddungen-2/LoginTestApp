namespace Server.Models
{
    public class Message
    {
        public long Id { get; set; }
        // Relationships
        public int SenderId { get; set; }
        public User Sender { get; set; } = null!;

        public int GroupId { get; set; }
        public ChatGroup Group { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsEncrypted { get; set; } = false;
    }
}