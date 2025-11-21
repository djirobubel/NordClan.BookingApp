using MediatR;
using Microsoft.AspNetCore.Mvc;
using NordClan.BookingApp.Api.CQRS.Commands.Login;
using NordClan.BookingApp.Api.Models;

namespace NordClan.BookingApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LoginController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Авторизация пользователя.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LoginRequest request)
        {
            var result = await _mediator.Send(new LoginCommand(request.Username, request.Password));
            return Ok(result);
        }
    }
}
