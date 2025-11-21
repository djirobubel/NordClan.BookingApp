using NordClan.BookingApp.Api.Models;
using NordClan.BookingApp.Api.Repository;

namespace NordClan.BookingApp.UnitTests.Repository
{
    public class BookingRepositoryTests
    {
        [Fact]
        public async Task GetBookingsAsync_ReturnsAll_WhenNoFilters()
        {
            // arrange
            using var context = TestDbContextFactory.CreateDbContext();

            context.Bookings.AddRange(
                new Booking
                {
                    Id = 1,
                    RoomId = 1,
                    UserLogin = "user1",
                    Title = "b1",
                    StartTime = new DateTime(2024, 1, 15, 9, 0, 0),
                    EndTime = new DateTime(2024, 1, 15, 10, 0, 0)
                },
                new Booking
                {
                    Id = 2,
                    RoomId = 2,
                    UserLogin = "user2",
                    Title = "b2",
                    StartTime = new DateTime(2024, 1, 16, 9, 0, 0),
                    EndTime = new DateTime(2024, 1, 16, 10, 0, 0)
                }
            );

            await context.SaveChangesAsync();

            var sut = new BookingRepository(context);

            // act
            var result = await sut.GetBookingsAsync(null, null);

            // assert
            var list = result.ToList();
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public async Task GetBookingsAsync_FiltersByRoomIdAndDate()
        {
            // arrange
            using var context = TestDbContextFactory.CreateDbContext();

            var date = new DateTime(2024, 1, 15);

            context.Bookings.AddRange(
                new Booking
                {
                    Id = 1,
                    RoomId = 1,
                    UserLogin = "user1",
                    Title = "room1 same day 1",
                    StartTime = date.AddHours(9),
                    EndTime = date.AddHours(10)
                },
                new Booking
                {
                    Id = 2,
                    RoomId = 1,
                    UserLogin = "user2",
                    Title = "room1 same day 2",
                    StartTime = date.AddHours(11),
                    EndTime = date.AddHours(12)
                },
                new Booking
                {
                    Id = 3,
                    RoomId = 2,
                    UserLogin = "user3",
                    Title = "room2 same day",
                    StartTime = date.AddHours(10),
                    EndTime = date.AddHours(11)
                },
                new Booking
                {
                    Id = 4,
                    RoomId = 1,
                    UserLogin = "user4",
                    Title = "room1 other day",
                    StartTime = date.AddDays(1).AddHours(9),
                    EndTime = date.AddDays(1).AddHours(10)
                }
            );

            await context.SaveChangesAsync();

            var sut = new BookingRepository(context);

            // act
            var result = await sut.GetBookingsAsync(roomId: 1, date: date);

            // assert
            var list = result.ToList();
            Assert.Equal(2, list.Count);
            Assert.All(list, b =>
            {
                Assert.Equal(1, b.RoomId);
                Assert.Equal(date.Date, b.StartTime.Date);
            });
        }

        [Fact]
        public async Task CreateBookingAsync_AddsBookingToDatabase()
        {
            // arrange
            using var context = TestDbContextFactory.CreateDbContext();

            var sut = new BookingRepository(context);

            var roomId = 1;
            string? userLogin = "user1";
            var start = new DateTime(2024, 1, 15, 9, 0, 0);
            var end = new DateTime(2024, 1, 15, 10, 0, 0);
            var title = "meeting";
            var description = "desc";

            // act
            await sut.CreateBookingAsync(roomId, userLogin, start, end, title, description);

            // assert
            var all = context.Bookings.ToList();
            Assert.Single(all);

            var b = all.First();
            Assert.Equal(roomId, b.RoomId);
            Assert.Equal(userLogin, b.UserLogin);
            Assert.Equal(start, b.StartTime);
            Assert.Equal(end, b.EndTime);
            Assert.Equal(title, b.Title);
            Assert.Equal(description, b.Description);
        }

        [Fact]
        public async Task CreateBookingAsync_SetsUnknownUser_WhenUserLoginIsNull()
        {
            // arrange
            using var context = TestDbContextFactory.CreateDbContext();

            var sut = new BookingRepository(context);

            // userLogin = null
            var roomId = 1;
            string? userLogin = null;
            var start = new DateTime(2024, 1, 15, 9, 0, 0);
            var end = new DateTime(2024, 1, 15, 10, 0, 0);
            var title = "meeting";
            var description = "desc";

            // act
            await sut.CreateBookingAsync(roomId, userLogin, start, end, title, description);

            // assert
            var b = context.Bookings.Single();
            Assert.Equal("unknown", b.UserLogin);
        }

        [Fact]
        public async Task UpdateBookingAsync_UpdatesExistingBooking()
        {
            // arrange
            using var context = TestDbContextFactory.CreateDbContext();

            var existing = new Booking
            {
                Id = 1,
                RoomId = 1,
                UserLogin = "user",
                Title = "old",
                Description = "old",
                StartTime = new DateTime(2024, 1, 15, 9, 0, 0),
                EndTime = new DateTime(2024, 1, 15, 10, 0, 0)
            };

            context.Bookings.Add(existing);
            await context.SaveChangesAsync();

            var sut = new BookingRepository(context);

            var newRoomId = 2;
            var newStart = new DateTime(2024, 1, 16, 11, 0, 0);
            var newEnd = new DateTime(2024, 1, 16, 12, 0, 0);
            var newTitle = "new";
            var newDesc = "new desc";

            // act
            await sut.UpdateBookingAsync(
                bookingId: existing.Id,
                roomId: newRoomId,
                startTime: newStart,
                endTime: newEnd,
                title: newTitle,
                description: newDesc);

            // assert
            var updated = await context.Bookings.FindAsync(existing.Id);
            Assert.NotNull(updated);
            Assert.Equal(newRoomId, updated!.RoomId);
            Assert.Equal(newStart, updated.StartTime);
            Assert.Equal(newEnd, updated.EndTime);
            Assert.Equal(newTitle, updated.Title);
            Assert.Equal(newDesc, updated.Description);
        }

        [Fact]
        public async Task UpdateBookingAsync_DoesNothing_WhenBookingDoesNotExist()
        {
            // arrange
            using var context = TestDbContextFactory.CreateDbContext();

            var sut = new BookingRepository(context);

            // act
            await sut.UpdateBookingAsync(
                bookingId: 999,
                roomId: 1,
                startTime: DateTime.Now,
                endTime: DateTime.Now.AddHours(1),
                title: "title",
                description: "desc");

            // assert
            Assert.Empty(context.Bookings);
        }

        [Fact]
        public async Task DeleteBookingAsync_RemovesBookingFromDatabase_WhenExists()
        {
            // arrange
            using var context = TestDbContextFactory.CreateDbContext();

            var booking = new Booking
            {
                Id = 1,
                RoomId = 1,
                UserLogin = "user",
                Title = "to delete",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1)
            };

            context.Bookings.Add(booking);
            await context.SaveChangesAsync();

            var sut = new BookingRepository(context);

            // act
            await sut.DeleteBookingAsync(booking.Id);

            // assert
            Assert.Empty(context.Bookings);
        }

        [Fact]
        public async Task DeleteBookingAsync_DoesNothing_WhenBookingDoesNotExist()
        {
            // arrange
            using var context = TestDbContextFactory.CreateDbContext();

            var sut = new BookingRepository(context);

            // act
            await sut.DeleteBookingAsync(999);

            // assert
            Assert.Empty(context.Bookings);
        }

        [Fact]
        public async Task HasOverlapAsync_ReturnsTrue_WhenOverlappingBookingExists()
        {
            // arrange
            using var context = TestDbContextFactory.CreateDbContext();

            var roomId = 1;
            var existing = new Booking
            {
                Id = 1,
                RoomId = roomId,
                UserLogin = "user",
                Title = "existing",
                StartTime = new DateTime(2024, 1, 15, 10, 0, 0),
                EndTime = new DateTime(2024, 1, 15, 11, 0, 0)
            };

            context.Bookings.Add(existing);
            await context.SaveChangesAsync();

            var sut = new BookingRepository(context);

            var newStart = new DateTime(2024, 1, 15, 10, 30, 0);
            var newEnd = new DateTime(2024, 1, 15, 11, 30, 0);

            // act
            var result = await sut.HasOverlapAsync(roomId, newStart, newEnd, excludeId: null);

            // assert
            Assert.True(result);
        }

        [Fact]
        public async Task HasOverlapAsync_ReturnsFalse_WhenNoOverlap()
        {
            // arrange
            using var context = TestDbContextFactory.CreateDbContext();

            var roomId = 1;
            var existing = new Booking
            {
                Id = 1,
                RoomId = roomId,
                UserLogin = "user",
                Title = "existing",
                StartTime = new DateTime(2024, 1, 15, 8, 0, 0),
                EndTime = new DateTime(2024, 1, 15, 9, 0, 0)
            };

            context.Bookings.Add(existing);
            await context.SaveChangesAsync();

            var sut = new BookingRepository(context);

            var newStart = new DateTime(2024, 1, 15, 10, 0, 0);
            var newEnd = new DateTime(2024, 1, 15, 11, 0, 0);

            // act
            var result = await sut.HasOverlapAsync(roomId, newStart, newEnd, excludeId: null);

            // assert
            Assert.False(result);
        }

        [Fact]
        public async Task HasOverlapAsync_IgnoresBookingWithExcludeId()
        {
            // arrange
            using var context = TestDbContextFactory.CreateDbContext();

            var roomId = 1;
            var existing = new Booking
            {
                Id = 10,
                RoomId = roomId,
                UserLogin = "user",
                Title = "existing",
                StartTime = new DateTime(2024, 1, 15, 10, 0, 0),
                EndTime = new DateTime(2024, 1, 15, 11, 0, 0)
            };

            context.Bookings.Add(existing);
            await context.SaveChangesAsync();

            var sut = new BookingRepository(context);

            var newStart = new DateTime(2024, 1, 15, 10, 0, 0);
            var newEnd = new DateTime(2024, 1, 15, 11, 0, 0);

            // act
            var result = await sut.HasOverlapAsync(roomId, newStart, newEnd, excludeId: 10);

            // assert
            Assert.False(result);
        }
    }
}
