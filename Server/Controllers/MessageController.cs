using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Shared.Models;
using System.Security.Claims;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MessagesController : ControllerBase
    {
        private readonly MessageService _service;

        public MessagesController(MessageService service)
        {
            _service = service;
        }

        [HttpPost("Send")]
        public async Task<ActionResult<MessageDto>> SendMessage([FromBody] MessageDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest("Missing content");
            try
            {
                dto.SenderId = GetUserIdFromJwt();
                var message = await _service.SendMessageAsync(dto.SenderId, dto.GroupId, dto.Content);
                return Ok(message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }
        [HttpGet("JoinedGroups")]
        public async Task<ActionResult<List<ChatGroupDto>>> GetMyGroups()
        {
            try
            {
                int userId = GetUserIdFromJwt();
                var groups = await _service.GetChatGroupsForUserAsync(userId);
                return Ok(groups);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
}

        [HttpGet("Group/{id}")]
        public async Task<ActionResult<List<MessageDto>>> GetMessagesForUser(int id)
        {
            try
            {
                int userId = GetUserIdFromJwt();
                var messages = await _service.GetMessagesForUserAsync(userId, id);
                return Ok(messages);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        //[HttpGet("Id/{id}")]
        //public async Task<ActionResult<Message>> GetMessageById(int id)
        //{
        //    var message = await _service.GetMessageByIdAsync(id);
        //    if (message == null) return NotFound();
        //    return Ok(message);
        //}

        //-------------------- HELPERS --------------------
        private int GetUserIdFromJwt()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
            if (!int.TryParse(userIdClaim.Value, out int userId))
                throw new UnauthorizedAccessException();

            return userId;
        }
    }
}
