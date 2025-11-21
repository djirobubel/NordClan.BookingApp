namespace NordClan.BookingApp.Api.CQRS.Queries.GetBookings
{
    public class GetBookingsQueryResult
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string UserLogin { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
    }
}
