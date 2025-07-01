namespace WebApplication1.Models
{
    public class User
    {
        public long Id { get; set; }
        public long TelegramId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; } = "Client";
        public long ChatId { get; set; }
    }
}
