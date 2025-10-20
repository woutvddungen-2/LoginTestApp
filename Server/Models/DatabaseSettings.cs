namespace Server.Models
{
    public class DatabaseSettings
    {
        public string Ip { get; set; } = string.Empty;
        public int Port { get; set; }
        public string DatabaseName { get; set; } = string.Empty;
        public string UserID { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    
}
