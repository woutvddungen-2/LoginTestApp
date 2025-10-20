namespace Server.Models
{
    public class Message
    {
        public long Id { get; set; }
        public int SenderId { get; set; }
        public int GroupId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsEncrypted { get; set; } = false;
    }
}