using Microsoft.EntityFrameworkCore;
using NordClan.BookingApp.Api.Data;
using NordClan.BookingApp.Api.Interface;
using NordClan.BookingApp.Api.Models;

namespace NordClan.BookingApp.Api.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingDbContext _context;

        public BookingRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetBookingsAsync(int? roomId, DateTime? date)
        {
            var bookings = _context.Bookings.AsQueryable();

            if (roomId.HasValue)
                bookings = bookings.Where(x => x.RoomId == roomId.Value);

            if (date.HasValue)
            {
                var start = date.Value.Date;
                var end = start.AddDays(1);
                bookings = bookings.Where(y => y.StartTime >= start && y.EndTime <= end);
            }

            return await bookings.ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int bookingId)
        {
            return await _context.Bookings.FindAsync(bookingId);
        }

        public async Task CreateBookingAsync(int roomId, string? userLogin, DateTime startTime, DateTime endTime, string title, string? description)
        {
            var booking = new Booking
            {
                RoomId = roomId,
                UserLogin = userLogin ?? "unknown",
                StartTime = startTime,
                EndTime = endTime,
                Title = title,
                Description = description
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBookingAsync(int bookingId, int roomId, DateTime startTime, DateTime endTime, string title, string? description)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
                return;

            booking.RoomId = roomId;
            booking.StartTime = startTime;
            booking.EndTime = endTime;
            booking.Title = title;
            booking.Description = description;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
                return;

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasOverlapAsync(int roomId, DateTime start, DateTime end, int? excludeId = null)
        {
            return await _context.Bookings.AnyAsync(b =>
                b.RoomId == roomId &&
                (excludeId == null || b.Id != excludeId) &&
                b.StartTime < end &&
                b.EndTime > start);
        }
    }
}
