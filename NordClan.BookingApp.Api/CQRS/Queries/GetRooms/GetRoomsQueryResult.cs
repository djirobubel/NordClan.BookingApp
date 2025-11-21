namespace NordClan.BookingApp.Api.CQRS.Queries.GetRooms
{
    public class GetRoomsQueryResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Colour { get; set; } = null!;
    }
}
