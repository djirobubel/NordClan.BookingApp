using Moq;
using NordClan.BookingApp.Api.Exceptions;
using NordClan.BookingApp.Api.Interface;
using NordClan.BookingApp.Api.Models;
using NordClan.BookingApp.Api.Service;

namespace NordClan.BookingApp.UnitTests.Service
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _bookingRepositoryMock;
        private readonly BookingService _sut;

        public BookingServiceTests()
        {
            _bookingRepositoryMock = new Mock<IBookingRepository>();
            _sut = new BookingService(_bookingRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateBookingAsync_CreatesBooking_WhenValidAndNoOverlap()
        {
            // arrange
            var request = new BookingRequest
            {
                RoomId = 1,
                Title = "title",
                StartTime = new DateTime(2024, 1, 15, 10, 0, 0),
                EndTime = new DateTime(2024, 1, 15, 11, 0, 0),
                Description = "description"
            };
            var userLogin = "userLogin";

            _bookingRepositoryMock
                .Setup(x => x.HasOverlapAsync(
                    request.RoomId,
                    request.StartTime,
                    request.EndTime,
                    null))
                .ReturnsAsync(false);

            _bookingRepositoryMock
                .Setup(x => x.CreateBookingAsync(
                    request.RoomId,
                    userLogin,
                    request.StartTime,
                    request.EndTime,
                    request.Title,
                    request.Description))
                .Returns(Task.CompletedTask);

            var createdBooking = new Booking
            {
                Id = 10,
                RoomId = request.RoomId,
                Title = request.Title,
                Description = request.Description,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                UserLogin = userLogin
            };

            var otherBooking = new Booking
            {
                Id = 5,
                RoomId = request.RoomId,
                Title = "other",
                Description = "other",
                StartTime = request.StartTime.AddHours(-2),
                EndTime = request.EndTime.AddHours(-2),
                UserLogin = "someone"
            };

            _bookingRepositoryMock
                .Setup(x => x.GetBookingsAsync(request.RoomId, null))
                .ReturnsAsync(new List<Booking> { otherBooking, createdBooking });

            // act
            var result = await _sut.CreateBookingAsync(request, userLogin);

            // assert
            Assert.NotNull(result);
            Assert.IsType<Booking>(result);
            Assert.Equal(createdBooking.Id, result.Id);
            Assert.Equal(createdBooking.RoomId, result.RoomId);
            Assert.Equal(createdBooking.Title, result.Title);
            Assert.Equal(createdBooking.Description, result.Description);
            Assert.Equal(createdBooking.StartTime, result.StartTime);
            Assert.Equal(createdBooking.EndTime, result.EndTime);
            Assert.Equal(createdBooking.UserLogin, result.UserLogin);

            _bookingRepositoryMock.Verify(x => x.CreateBookingAsync(
                request.RoomId,
                userLogin,
                request.StartTime,
                request.EndTime,
                request.Title,
                request.Description), Times.Once);
        }

        [Fact]
        public async Task CreateBookingAsync_ThrowsBookingValidationException_WhenDurationIsInvalid()
        {
            // arrange
            var request = new BookingRequest
            {
                RoomId = 1,
                Title = "short",
                StartTime = new DateTime(2024, 1, 15, 10, 0, 0),
                EndTime = new DateTime(2024, 1, 15, 10, 10, 0), 
                Description = "desc"
            };
            var userLogin = "userLogin";

            // act
            var ex = await Assert.ThrowsAsync<BookingValidationException>(
                () => _sut.CreateBookingAsync(request, userLogin));

            // assert
            Assert.Equal("Длительность брони должна быть от 30 минут до 8 часов!", ex.Message);

            _bookingRepositoryMock.Verify(
                x => x.CreateBookingAsync(It.IsAny<int>(), It.IsAny<string>(),
                    It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string?>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateBookingAsync_ThrowsBookingOverlapException_WhenHasOverlap()
        {
            // arrange
            var request = new BookingRequest
            {
                RoomId = 1,
                Title = "title",
                StartTime = new DateTime(2024, 1, 15, 10, 0, 0),
                EndTime = new DateTime(2024, 1, 15, 11, 0, 0),
                Description = "description"
            };
            var userLogin = "userLogin";

            _bookingRepositoryMock
                .Setup(x => x.HasOverlapAsync(
                    request.RoomId,
                    request.StartTime,
                    request.EndTime,
                    null))
                .ReturnsAsync(true);

            // act
            var ex = await Assert.ThrowsAsync<BookingOverlapException>(
                () => _sut.CreateBookingAsync(request, userLogin));

            // assert
            Assert.Equal("Это время уже забронировано в выбранной комнате!", ex.Message);

            _bookingRepositoryMock.Verify(
                x => x.CreateBookingAsync(It.IsAny<int>(), It.IsAny<string>(),
                    It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string?>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateBookingAsync_UpdatesBooking_WhenOwnerAndNoOverlap()
        {
            // arrange
            var bookingId = 42;
            var userLogin = "owner";

            var existing = new Booking
            {
                Id = bookingId,
                RoomId = 1,
                Title = "old",
                Description = "old",
                StartTime = new DateTime(2024, 1, 15, 9, 0, 0),
                EndTime = new DateTime(2024, 1, 15, 10, 0, 0),
                UserLogin = userLogin
            };

            var request = new BookingRequest
            {
                RoomId = 1,
                Title = "new title",
                Description = "new desc",
                StartTime = new DateTime(2024, 1, 15, 11, 0, 0),
                EndTime = new DateTime(2024, 1, 15, 12, 0, 0)
            };

            _bookingRepositoryMock
                .SetupSequence(x => x.GetByIdAsync(bookingId))
                .ReturnsAsync(existing)
                .ReturnsAsync(new Booking
                {
                    Id = bookingId,
                    RoomId = request.RoomId,
                    Title = request.Title,
                    Description = request.Description,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    UserLogin = userLogin
                }); 

            _bookingRepositoryMock
                .Setup(x => x.HasOverlapAsync(
                    request.RoomId,
                    request.StartTime,
                    request.EndTime,
                    bookingId))
                .ReturnsAsync(false);

            _bookingRepositoryMock
                .Setup(x => x.UpdateBookingAsync(
                    bookingId,
                    request.RoomId,
                    request.StartTime,
                    request.EndTime,
                    request.Title,
                    request.Description))
                .Returns(Task.CompletedTask);

            // act
            var result = await _sut.UpdateBookingAsync(bookingId, request, userLogin);

            // assert
            Assert.NotNull(result);
            Assert.Equal(bookingId, result.Id);
            Assert.Equal(request.RoomId, result.RoomId);
            Assert.Equal(request.Title, result.Title);
            Assert.Equal(request.Description, result.Description);
            Assert.Equal(request.StartTime, result.StartTime);
            Assert.Equal(request.EndTime, result.EndTime);
            Assert.Equal(userLogin, result.UserLogin);
        }

        [Fact]
        public async Task UpdateBookingAsync_ThrowsBookingNotFoundException_WhenBookingDoesNotExist()
        {
            // arrange
            var bookingId = 42;
            var request = new BookingRequest
            {
                RoomId = 1,
                Title = "title",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1)
            };
            var userLogin = "user";

            _bookingRepositoryMock
                .Setup(x => x.GetByIdAsync(bookingId))
                .ReturnsAsync((Booking?)null);

            // act
            var ex = await Assert.ThrowsAsync<BookingNotFoundException>(
                () => _sut.UpdateBookingAsync(bookingId, request, userLogin));

            // assert
            Assert.Equal($"Бронирование с id={bookingId} не найдено.", ex.Message);
        }

        [Fact]
        public async Task UpdateBookingAsync_ThrowsBookingAccessDeniedException_WhenUserIsNotOwner()
        {
            // arrange
            var bookingId = 42;

            var existing = new Booking
            {
                Id = bookingId,
                RoomId = 1,
                Title = "old",
                Description = "old",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                UserLogin = "owner"
            };

            var request = new BookingRequest
            {
                RoomId = 1,
                Title = "new",
                Description = "new",
                StartTime = DateTime.Now.AddHours(2),
                EndTime = DateTime.Now.AddHours(3)
            };

            var otherUser = "other";

            _bookingRepositoryMock
                .Setup(x => x.GetByIdAsync(bookingId))
                .ReturnsAsync(existing);

            // act
            var ex = await Assert.ThrowsAsync<BookingAccessDeniedException>(
                () => _sut.UpdateBookingAsync(bookingId, request, otherUser));

            // assert
            Assert.Equal("Вы не можете редактировать чужое бронирование.", ex.Message);
        }

        [Fact]
        public async Task DeleteBookingAsync_Deletes_WhenOwner()
        {
            // arrange
            var bookingId = 42;
            var userLogin = "owner";

            var existing = new Booking
            {
                Id = bookingId,
                RoomId = 1,
                UserLogin = userLogin,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                Title = "title"
            };

            _bookingRepositoryMock
                .Setup(x => x.GetByIdAsync(bookingId))
                .ReturnsAsync(existing);

            _bookingRepositoryMock
                .Setup(x => x.DeleteBookingAsync(bookingId))
                .Returns(Task.CompletedTask);

            // act
            await _sut.DeleteBookingAsync(bookingId, userLogin);

            // assert
            _bookingRepositoryMock.Verify(x => x.DeleteBookingAsync(bookingId), Times.Once);
        }

        [Fact]
        public async Task DeleteBookingAsync_ThrowsBookingNotFoundException_WhenNotExists()
        {
            // arrange
            var bookingId = 42;
            var userLogin = "user";

            _bookingRepositoryMock
                .Setup(x => x.GetByIdAsync(bookingId))
                .ReturnsAsync((Booking?)null);

            // act
            var ex = await Assert.ThrowsAsync<BookingNotFoundException>(
                () => _sut.DeleteBookingAsync(bookingId, userLogin));

            // assert
            Assert.Equal($"Бронирование с id={bookingId} не найдено.", ex.Message);
        }

        [Fact]
        public async Task DeleteBookingAsync_ThrowsBookingAccessDeniedException_WhenUserIsNotOwner()
        {
            // arrange
            var bookingId = 42;

            var existing = new Booking
            {
                Id = bookingId,
                RoomId = 1,
                UserLogin = "owner",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                Title = "title"
            };

            var otherUser = "other";

            _bookingRepositoryMock
                .Setup(x => x.GetByIdAsync(bookingId))
                .ReturnsAsync(existing);

            // act
            var ex = await Assert.ThrowsAsync<BookingAccessDeniedException>(
                () => _sut.DeleteBookingAsync(bookingId, otherUser));

            // assert
            Assert.Equal("Вы не можете удалить чужое бронирование.", ex.Message);
        }
    }
}
