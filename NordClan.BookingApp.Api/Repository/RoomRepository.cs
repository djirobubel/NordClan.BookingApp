using Microsoft.EntityFrameworkCore;
using NordClan.BookingApp.Api.Data;
using NordClan.BookingApp.Api.Interface;
using NordClan.BookingApp.Api.Models;

namespace NordClan.BookingApp.Api.Repository
{
    public class RoomRepository : IRoomRepository
    {
        private readonly BookingDbContext _context;

        public RoomRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetRoomsAsync()
        {
            return await _context.Rooms.ToListAsync();
        }
    }
}
