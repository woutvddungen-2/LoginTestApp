namespace Server.Models
{
    public class GroupMember
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public Group Group { get; set; } = null!;
    }
}
