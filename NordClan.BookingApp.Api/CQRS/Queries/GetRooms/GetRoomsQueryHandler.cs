using MediatR;
using NordClan.BookingApp.Api.Interface;

namespace NordClan.BookingApp.Api.CQRS.Queries.GetRooms
{
    public class GetRoomsQueryHandler : IRequestHandler<GetRoomsQuery, IEnumerable<GetRoomsQueryResult>>
    {
        private readonly IRoomRepository _roomRepository;

        public GetRoomsQueryHandler(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<IEnumerable<GetRoomsQueryResult>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
        {
            var rooms = await _roomRepository.GetRoomsAsync();

            var result = rooms.Select(x => new GetRoomsQueryResult
            {
                Id = x.Id,
                Name = x.Name,
                Colour = x.Colour
            }).ToList();

            return result;
        }
    }
}
