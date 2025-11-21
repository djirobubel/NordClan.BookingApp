using MediatR;
using NordClan.BookingApp.Api.Interface;

namespace NordClan.BookingApp.Api.CQRS.Queries.GetBookings
{
    public class GetBookingsQueryHandler : IRequestHandler<GetBookingsQuery, IEnumerable<GetBookingsQueryResult>>
    {
        private readonly IBookingService _bookingService;

        public GetBookingsQueryHandler(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task<IEnumerable<GetBookingsQueryResult>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
        {
            var bookings = await _bookingService.GetBookingsAsync(request.RoomId, request.Date);

            var result = bookings.Select(x => new GetBookingsQueryResult
            {
                Id = x.Id,
                RoomId = x.RoomId,
                UserLogin = x.UserLogin,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                Title = x.Title,
                Description = x.Description
            }).ToList();

            return result;
        }
    }
}
