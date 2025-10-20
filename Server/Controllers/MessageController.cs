using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.Models;
using Shared.Models;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly MessageService _service;

        public MessagesController(MessageService service)
        {
            _service = service;
        }

        [HttpPost("Send")]
        public async Task<ActionResult<Message>> SendMessage([FromBody] MessageDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest("Missing content");
            try
            {
                int userId = GetUserIdFromJwt();
                var message = await _service.SendMessageAsync(userId, dto.GroupId, dto.Content);
                return Ok(message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpGet("Group/{id}")]
        public async Task<ActionResult<List<Message>>> GetMessagesForUser(int groupId)
        {
            try
            {
                int userId = GetUserIdFromJwt();
                var messages = await _service.GetMessagesForUserAsync(userId, groupId);
                return Ok(messages);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpGet("Id/{id}")]
        public async Task<ActionResult<Message>> GetMessageById(int id)
        {
            var message = await _service.GetMessageByIdAsync(id);
            if (message == null) return NotFound();
            return Ok(message);
        }

        //-------------------- HELPERS --------------------
        private int GetUserIdFromJwt()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User ID not found in token.");

            return int.Parse(userIdClaim.Value);
        }


    }
}
