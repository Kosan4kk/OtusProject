namespace WebApplication1.Models
{
    public class StudioBooking
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int RoomId { get; set; }
        public StudioRoom Room { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }

        public List<Participant> Participants { get; set; } = new List<Participant>();
    }
}
