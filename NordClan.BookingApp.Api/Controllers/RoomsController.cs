using MediatR;
using Microsoft.AspNetCore.Mvc;
using NordClan.BookingApp.Api.CQRS.Queries.GetRooms;
using NordClan.BookingApp.Api.Interface;

namespace NordClan.BookingApp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RoomsController(IRoomRepository roomRepository, IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получение списка комнат.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetRoomsQueryResult>>> Get()
        {
            return Ok(await _mediator.Send(new GetRoomsQuery()));
        }
    }
}
