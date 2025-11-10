using NordClan.BookingApp.Api.Models;

namespace NordClan.BookingApp.Api.Data
{
    public static class FakeDatabase
    {
        public static List<Room> Rooms { get; } = new()
        {
            new() { Id = 1, Name = "Меркурий" },
            new() { Id = 2, Name = "Венера" },
            new() { Id = 3, Name = "Марс" },
            new() { Id = 4, Name = "Юпитер" },
            new() { Id = 5, Name = "Сатурн" }
        };

        public static List<Booking> Bookings { get; } = new()
        {
            new() { Id = 1, RoomId = 1, UserLogin = "ivanov", StartTime = new DateTime(2025, 11, 6, 9, 0, 0), EndTime = new DateTime(2025, 11, 6, 10, 0, 0), Title = "Синк с командой" },
            new() { Id = 2, RoomId = 1, UserLogin = "petrov", StartTime = new DateTime(2025, 11, 6, 14, 0, 0), EndTime = new DateTime(2025, 11, 6, 15, 30, 0), Title = "Демо клиенту" },
            new() { Id = 3, RoomId = 2, UserLogin = "sidorov", StartTime = new DateTime(2025, 11, 7, 11, 0, 0), EndTime = new DateTime(2025, 11, 7, 13, 0, 0), Title = "Планирование спринта" },
            new() { Id = 4, RoomId = 3, UserLogin = "guest", StartTime = new DateTime(2025, 11, 6, 16, 0, 0), EndTime = new DateTime(2025, 11, 6, 18, 0, 0), Title = "Вечерний брейншторм" }
        };

        private static int _nextBookingId = 5;
        public static int GetNextBookingId() => _nextBookingId++;
    }
}
