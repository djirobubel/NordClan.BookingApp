using MediatR;
using NordClan.BookingApp.Api.Models;

namespace NordClan.BookingApp.Api.CQRS.Commands.CreateBooking
{
    public class CreateBookingCommand : IRequest<CreateBookingCommandResult>
    {
        public string UserLogin { get; }
        public BookingRequest Request { get; }

        public CreateBookingCommand(string userLogin, BookingRequest request)
        {
            UserLogin = userLogin;
            Request = request;
        }
    }
}
