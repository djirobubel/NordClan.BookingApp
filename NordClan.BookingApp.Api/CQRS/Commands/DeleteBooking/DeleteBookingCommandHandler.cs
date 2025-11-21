using MediatR;
using NordClan.BookingApp.Api.Interface;

namespace NordClan.BookingApp.Api.CQRS.Commands.DeleteBooking
{
    public class DeleteBookingCommandHandler : IRequestHandler<DeleteBookingCommand, Unit>
    {
        private readonly IBookingService _bookingService;

        public DeleteBookingCommandHandler(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task<Unit> Handle(DeleteBookingCommand request, CancellationToken cancellationToken)
        {
            await _bookingService.DeleteBookingAsync(request.Id, request.UserLogin);

            return Unit.Value;
        }
    }
}
