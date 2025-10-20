using Microsoft.EntityFrameworkCore;
using Server.Models;

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
        public async Task<Message> SendMessageAsync(int senderId, int groupId, string content)
        {
            // Verify membership
            bool isMember = await _db.GroupMembers
                .AnyAsync(gm => gm.GroupId == groupId && gm.UserId == senderId);

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
            return message;
        }

        /// <summary>
        /// Retrieves a list of messages for a specific user within a specified group.
        /// </summary>
        public async Task<List<Message>> GetMessagesForUserAsync(int userId, int groupId)
        {
            bool isMember = await _db.GroupMembers
                .AnyAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (!isMember)
                throw new UnauthorizedAccessException("User is not a member of this group.");

            return await _db.Messages
                .Where(m => m.GroupId == groupId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }


        /// <summary>
        /// Retrieves a message by its unique identifier.
        /// </summary>
        public async Task<Message?> GetMessageByIdAsync(int id)
        {
            return await _db.Messages.FindAsync(id);
        }
    }
}
