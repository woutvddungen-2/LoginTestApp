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
        public int GroupId { get; set; }
        public string Content { get; set; } = null!;        
    }
}
