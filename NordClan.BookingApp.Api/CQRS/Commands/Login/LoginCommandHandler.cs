using MediatR;
using NordClan.BookingApp.Api.Exceptions;
using NordClan.BookingApp.Api.Interface;
using NordClan.BookingApp.Api.Models;

namespace NordClan.BookingApp.Api.CQRS.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly ILdapService _ldapService;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginCommandHandler(ILdapService ldapService, IJwtTokenService jwtTokenService)
        {
            _ldapService = ldapService;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                throw new LoginValidationException("Логин и пароль обязательны!");

            var isValid = await _ldapService.ValidateCredentialsAsync(request.Username, request.Password);

            if (!isValid)
                throw new InvalidCredentialsException("Неверный логин или пароль!");

            var token = _jwtTokenService.GenerateToken(request.Username);

            return new LoginResponse
            {
                Token = token,
                Username = request.Username
            };
        }
    }
}
