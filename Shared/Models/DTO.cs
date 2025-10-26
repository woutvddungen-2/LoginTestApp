using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class ProductDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class MessageDto
    {
        //public int Id { get; set; }              // optional
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public int GroupId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
        public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        //public string Email { get; set; } = string.Empty;
        //public DateTime CreatedAt { get; set; }
    }
    public class ChatGroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
