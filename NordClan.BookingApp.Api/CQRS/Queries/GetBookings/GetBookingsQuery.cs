using MediatR;

namespace NordClan.BookingApp.Api.CQRS.Queries.GetBookings
{
    public class GetBookingsQuery : IRequest<IEnumerable<GetBookingsQueryResult>>
    {
        public int? RoomId { get; }
        public DateTime? Date { get; }

        public GetBookingsQuery(int? roomId, DateTime? date)
        {
            RoomId = roomId;
            Date = date;
        }
    }
}
