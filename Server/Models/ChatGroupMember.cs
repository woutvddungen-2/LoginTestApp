namespace Server.Models
{
    public class ChatGroupMember
    {
        public int ChatGroupId { get; set; }
        public int UserId { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.Now;
        public User User { get; set; } = null!;
        public ChatGroup ChatGroup { get; set; } = null!;
    }
}
