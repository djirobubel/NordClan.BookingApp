using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NordClan.BookingApp.Api.Data;
using NordClan.BookingApp.Api.Models;

namespace NordClan.BookingApp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly BookingDbContext _context;

        public BookingsController(BookingDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> Get([FromQuery] int? roomId, [FromQuery] DateTime? date)
        {
            var query = _context.Bookings.AsQueryable();

            if (roomId.HasValue)
                query = query.Where(x => x.RoomId == roomId.Value);

            if (date.HasValue)
            {
                var start = date.Value.Date;
                var end = start.AddDays(1);
                query = query.Where(y => y.StartTime >= start && y.EndTime <= end);
            }

            return Ok(await query.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookingRequest bookingRequest)
        {
            if (!IsValidDuration(bookingRequest.StartTime, bookingRequest.EndTime))
                return BadRequest("Длительность брони должна быть от 30 минут до 8 часов!");

            if (await HasOverlapAsync(bookingRequest.RoomId, bookingRequest.StartTime, bookingRequest.EndTime))
                return Conflict("Это время уже забронировано в выбранной комнате!");

            var booking = new Booking
            {
                RoomId = bookingRequest.RoomId,
                UserLogin = User.Identity?.Name ?? "unknwon",
                StartTime = bookingRequest.StartTime,
                EndTime = bookingRequest.EndTime,
                Title = bookingRequest.Title,
                Description = bookingRequest.Description
            }; 

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return Ok(booking);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] BookingRequest bookingRequest)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            if (!IsValidDuration(bookingRequest.StartTime, bookingRequest.EndTime))
                return BadRequest("Длительность брони должна быть от 30 минут до 8 часов!");

            if (await HasOverlapAsync(bookingRequest.RoomId, bookingRequest.StartTime, bookingRequest.EndTime, id))
                return Conflict("Это время уже забронировано в выбранной комнате!");

            booking.RoomId = bookingRequest.RoomId;
            booking.StartTime = bookingRequest.StartTime;
            booking.EndTime = bookingRequest.EndTime;
            booking.Title = bookingRequest.Title;
            booking.Description = bookingRequest.Description;

            await _context.SaveChangesAsync();
            return Ok(booking);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        #region Validation

        private static bool IsValidDuration(DateTime start, DateTime end)
        {
            var duration = (end - start).TotalMinutes;
            return duration >= 30 && duration <= 480;
        }

        private async Task<bool> HasOverlapAsync(int roomId, DateTime start, DateTime end, int? excludeId = null)
        {
            return await _context.Bookings.AnyAsync(b =>
                b.RoomId == roomId &&
                (excludeId == null || b.Id != excludeId) &&
                b.StartTime < end &&
                b.EndTime > start);
        }

        #endregion
    }
}
