namespace Server.Models
{
    public class ChatGroup
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ChatGroupMember> Members { get; set; } = new List<ChatGroupMember>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
