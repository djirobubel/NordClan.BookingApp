using MediatR;
using NordClan.BookingApp.Api.Interface;

namespace NordClan.BookingApp.Api.CQRS.Commands.UpdateBooking
{
    public class UpdateBookingCommandHandler : IRequestHandler<UpdateBookingCommand, UpdateBookingCommandResult>
    {
        private readonly IBookingService _bookingService;

        public UpdateBookingCommandHandler(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task<UpdateBookingCommandResult> Handle(UpdateBookingCommand request, CancellationToken cancellationToken)
        {
            var updated = await _bookingService.UpdateBookingAsync(request.Id, request.Request, request.UserLogin);

            var result = new UpdateBookingCommandResult
            {
                Id = updated.Id,
                RoomId = updated.RoomId,
                UserLogin = updated.UserLogin,
                StartTime = updated.StartTime,
                EndTime = updated.EndTime,
                Title = updated.Title,
                Description = updated.Description
            };

            return result;
        }
    }
}
