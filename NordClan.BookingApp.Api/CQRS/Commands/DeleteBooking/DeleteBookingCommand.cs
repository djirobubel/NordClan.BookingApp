using MediatR;

namespace NordClan.BookingApp.Api.CQRS.Commands.DeleteBooking
{
    public class DeleteBookingCommand : IRequest<Unit>
    {
        public int Id { get; }
        public string UserLogin { get; }

        public DeleteBookingCommand(int id, string userLogin)
        {
            Id = id;
            UserLogin = userLogin;
        }
    }
}
