using NordClan.BookingApp.Api.Exceptions;
using NordClan.BookingApp.Api.Interface;
using NordClan.BookingApp.Api.Models;

namespace NordClan.BookingApp.Api.Service
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<IEnumerable<Booking>> GetBookingsAsync(int? roomId, DateTime? date)
        {
            return await _bookingRepository.GetBookingsAsync(roomId, date);
        }

        public async Task<Booking> CreateBookingAsync(BookingRequest request, string userLogin)
        {
            if (!IsValidDuration(request.StartTime, request.EndTime))
                throw new BookingValidationException("Длительность брони должна быть от 30 минут до 8 часов!");

            var hasOverlap = await _bookingRepository.HasOverlapAsync(
                request.RoomId,
                request.StartTime,
                request.EndTime,
                excludeId: null);

            if (hasOverlap)
                throw new BookingOverlapException("Это время уже забронировано в выбранной комнате!");

            await _bookingRepository.CreateBookingAsync(
                request.RoomId,
                userLogin,
                request.StartTime,
                request.EndTime,
                request.Title,
                request.Description
            );

            var bookings = await _bookingRepository.GetBookingsAsync(request.RoomId, null);
            var created = bookings
                .OrderByDescending(b => b.Id)
                .FirstOrDefault(b =>
                    b.RoomId == request.RoomId &&
                    b.StartTime == request.StartTime &&
                    b.EndTime == request.EndTime &&
                    b.Title == request.Title);

            return created!;
        }

        public async Task<Booking> UpdateBookingAsync(int bookingId, BookingRequest request, string userLogin)
        {
            var existing = await _bookingRepository.GetByIdAsync(bookingId);
            if (existing == null)
                throw new BookingNotFoundException($"Бронирование с id={bookingId} не найдено.");

            if(!string.Equals(existing.UserLogin, userLogin, StringComparison.OrdinalIgnoreCase))
                throw new BookingAccessDeniedException("Вы не можете редактировать чужое бронирование.");

            if (!IsValidDuration(request.StartTime, request.EndTime))
                throw new BookingValidationException("Длительность брони должна быть от 30 минут до 8 часов!");

            var hasOverlap = await _bookingRepository.HasOverlapAsync(
                request.RoomId,
                request.StartTime,
                request.EndTime,
                excludeId: bookingId);

            if (hasOverlap)
                throw new BookingOverlapException("Это время уже забронировано в выбранной комнате!");

            await _bookingRepository.UpdateBookingAsync(
                bookingId,
                request.RoomId,
                request.StartTime,
                request.EndTime,
                request.Title,
                request.Description
            );

            var updated = await _bookingRepository.GetByIdAsync(bookingId);
            return updated!;
        }

        public async Task DeleteBookingAsync(int bookingId, string userLogin)
        {
            var existing = await _bookingRepository.GetByIdAsync(bookingId);
            if (existing == null)
                throw new BookingNotFoundException($"Бронирование с id={bookingId} не найдено.");

            if(!string.Equals(existing.UserLogin, userLogin, StringComparison.OrdinalIgnoreCase))
                throw new BookingAccessDeniedException("Вы не можете удалить чужое бронирование.");

            await _bookingRepository.DeleteBookingAsync(bookingId);
        }

        #region Validation

        private static bool IsValidDuration(DateTime start, DateTime end)
        {
            var duration = (end - start).TotalMinutes;
            return duration >= 30 && duration <= 480;
        }

        #endregion
    }
}
