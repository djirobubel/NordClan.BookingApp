namespace NordClan.BookingApp.Api.Models
{
    public class BookingRequest
    {
        public int RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
    }
}
