using MediatR;
using NordClan.BookingApp.Api.Models;

namespace NordClan.BookingApp.Api.CQRS.Commands.UpdateBooking
{
    public class UpdateBookingCommand : IRequest<UpdateBookingCommandResult>
    {
        public int Id { get; }
        public BookingRequest Request { get; }
        public string UserLogin { get; }

        public UpdateBookingCommand(int id, BookingRequest request, string userLogin)
        {
            Id = id;
            Request = request;
            UserLogin = userLogin;
        }
    }
}
