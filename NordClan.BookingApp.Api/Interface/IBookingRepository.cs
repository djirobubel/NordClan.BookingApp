using NordClan.BookingApp.Api.Models;

namespace NordClan.BookingApp.Api.Interface
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetBookingsAsync(int? roomId, DateTime? date);

        Task<Booking?> GetByIdAsync(int bookingId);

        Task CreateBookingAsync(int roomId, string userLogin, DateTime startTime, DateTime endTime, string title, string? description);

        Task UpdateBookingAsync(int bookingId, int roomId, DateTime startTime, DateTime endTime, string title, string? description);

        Task DeleteBookingAsync(int bookingId);

        Task<bool> HasOverlapAsync(int roomId, DateTime start, DateTime end, int? excludeId = null);
    }
}
