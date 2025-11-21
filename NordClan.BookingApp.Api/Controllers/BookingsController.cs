using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NordClan.BookingApp.Api.CQRS.Commands.CreateBooking;
using NordClan.BookingApp.Api.CQRS.Commands.DeleteBooking;
using NordClan.BookingApp.Api.CQRS.Commands.UpdateBooking;
using NordClan.BookingApp.Api.CQRS.Queries.GetBookings;
using NordClan.BookingApp.Api.Models;

namespace NordClan.BookingApp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получение списка бронирований с фильтрами.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<GetBookingsQueryResult>>> Get([FromQuery] int? roomId, [FromQuery] DateTime? date)
        {
            var result = await _mediator.Send(new GetBookingsQuery(roomId, date));
            return Ok(result);
        }

        /// <summary>
        /// Создание нового бронирования.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookingRequest bookingRequest)
        {
            var userLogin = User.Identity?.Name ?? "unknown";
            var result = await _mediator.Send(new CreateBookingCommand(userLogin, bookingRequest));
            return Ok(result);
        }

        /// <summary>
        /// Обновление существующего бронирования.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] BookingRequest bookingRequest)
        {
            var userLogin = User.Identity?.Name ?? "unknown";
            var result = await _mediator.Send(new UpdateBookingCommand(id, bookingRequest, userLogin));
            return Ok(result);
        }

        /// <summary>
        /// Удаление бронирования.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userLogin = User.Identity?.Name ?? "unknown";
            await _mediator.Send(new DeleteBookingCommand(id, userLogin));
            return NoContent();
        }
    }
}
