using MediatR;
using NordClan.BookingApp.Api.Interface;

namespace NordClan.BookingApp.Api.CQRS.Commands.CreateBooking
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, CreateBookingCommandResult>
    {
        private readonly IBookingService _bookingService;

        public CreateBookingCommandHandler(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task<CreateBookingCommandResult> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            var created = await _bookingService.CreateBookingAsync(request.Request, request.UserLogin);

            var result = new CreateBookingCommandResult
            {
                Id = created.Id,
                RoomId = created.RoomId,
                UserLogin = created.UserLogin,
                StartTime = created.StartTime,
                EndTime = created.EndTime,
                Title = created.Title,
                Description = created.Description
            };

            return result;
        }
    }
}
