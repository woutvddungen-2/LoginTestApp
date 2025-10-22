namespace Server.Models
{
    public class ChatGroupMember
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.Now;
        public User User { get; set; } = null!;
        public ChatGroup Group { get; set; } = null!;
    }
}
