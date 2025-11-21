using MediatR;
using NordClan.BookingApp.Api.Models;

namespace NordClan.BookingApp.Api.CQRS.Commands.Login
{
    public class LoginCommand : IRequest<LoginResponse>
    {
        public string Username { get; }
        public string Password { get; }

        public LoginCommand(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
