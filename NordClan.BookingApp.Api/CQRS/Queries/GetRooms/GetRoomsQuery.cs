using MediatR;

namespace NordClan.BookingApp.Api.CQRS.Queries.GetRooms
{
    public class GetRoomsQuery : IRequest<IEnumerable<GetRoomsQueryResult>>
    {
    }
}
