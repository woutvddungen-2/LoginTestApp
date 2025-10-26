using Microsoft.EntityFrameworkCore;
using Server.Models;
using Shared.Models;

namespace Server.Data
{
    public class MessageService
    {
        private readonly AppDbContext _db;

        public MessageService(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Send a message to a group. Sender is validated via JWT.
        /// </summary>
        public async Task<MessageDto> SendMessageAsync(int senderId, int groupId, string content)
        {
            bool isMember = await _db.ChatGroupMembers
                .AnyAsync(gm => gm.ChatGroupId == groupId && gm.UserId == senderId);

            if (!isMember)
                throw new UnauthorizedAccessException("User is not a member of this group.");

            var message = new Message
            {
                SenderId = senderId,
                GroupId = groupId,
                Content = content
            };

            _db.Messages.Add(message);
            await _db.SaveChangesAsync();

            var sender = await _db.Users.FindAsync(senderId);

            return new MessageDto
            {
                //Id = (int)message.Id,
                SenderId = senderId,
                SenderName = sender?.Username ?? "Unknown",
                GroupId = groupId,
                Content = content,
                CreatedAt = message.CreatedAt
            };
        }

        /// <summary>
        /// Retrieves all chat groups that a specific user is a member of.
        /// </summary>
        /// 
        public async Task<List<ChatGroupDto>> GetChatGroupsForUserAsync(int userId)
        {
            return await _db.ChatGroupMembers
                .Where(gm => gm.UserId == userId)
                .Select(gm => new ChatGroupDto { Id = gm.ChatGroup.Id, Name = gm.ChatGroup.Name })
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a list of messages for a specific user within a specified group.
        /// </summary>
        public async Task<List<MessageDto>> GetMessagesForUserAsync(int userId, int groupId)
        {
            bool isMember = await _db.ChatGroupMembers
                .AnyAsync(gm => gm.ChatGroupId == groupId && gm.UserId == userId);

            if (!isMember)
                throw new UnauthorizedAccessException("User is not a member of this group.");

            return await _db.Messages
                .Where(m => m.GroupId == groupId)
                .OrderBy(m => m.CreatedAt)
                .Select(m => new MessageDto
                {
                    //Id = (int)m.Id,
                    SenderId = m.SenderId,
                    SenderName = m.Sender.Username,
                    GroupId = m.GroupId,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync();
        }
    }
}
