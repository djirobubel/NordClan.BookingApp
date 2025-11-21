using NordClan.BookingApp.Api.Models;

namespace NordClan.BookingApp.Api.Interface
{
    public interface IBookingService
    {
        Task<IEnumerable<Booking>> GetBookingsAsync(int? roomId, DateTime? date);

        Task<Booking> CreateBookingAsync(BookingRequest request, string userLogin);

        Task<Booking> UpdateBookingAsync(int bookingId, BookingRequest request, string userLogin);

        Task DeleteBookingAsync(int bookingId, string userLogin);
    }
}
