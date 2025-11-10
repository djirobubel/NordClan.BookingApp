using NordClan.BookingApp.Shared.Models;

namespace NordClan.BookingApp.Client.Services
{
    public interface IBookingService
    {
        Task<List<Booking>> GetBookingsAsync(int? roomId = null, DateTime? date = null);
        Task<Booking> CreateBookingAsync(BookingRequest request);
        Task UpdateBookingAsync(int id, BookingRequest request);
        Task DeleteBookingAsync(int id);
    }
}
