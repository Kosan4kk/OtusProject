namespace WebApplication1.Models
{
    public class Participant
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public string FullName { get; set; }

        public int BookingId { get; set; }
        public StudioBooking Booking { get; set; }
    }
}
