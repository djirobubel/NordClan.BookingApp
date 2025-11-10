using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NordClan.BookingApp.Api.Data;
using NordClan.BookingApp.Api.Models;

namespace NordClan.BookingApp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly BookingDbContext _context;

        public RoomsController(BookingDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> Get()
        {
            return Ok(await _context.Rooms.ToListAsync());
        }
    }
}
