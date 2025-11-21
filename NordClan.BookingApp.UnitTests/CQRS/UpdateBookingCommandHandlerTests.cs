using Moq;
using NordClan.BookingApp.Api.CQRS.Commands.UpdateBooking;
using NordClan.BookingApp.Api.Interface;
using NordClan.BookingApp.Api.Models;

namespace NordClan.BookingApp.UnitTests.CQRS
{
    public class UpdateBookingCommandHandlerTests
    {
        [Fact]
        public async Task Handle_CallsService_AndReturnsMappedResult()
        {
            // arrange
            var bookingId = 42;
            var userLogin = "user1";

            var request = new BookingRequest
            {
                RoomId = 2,
                Title = "updated",
                Description = "updated desc",
                StartTime = new DateTime(2024, 1, 16, 11, 0, 0),
                EndTime = new DateTime(2024, 1, 16, 12, 0, 0)
            };

            var updated = new Booking
            {
                Id = bookingId,
                RoomId = request.RoomId,
                Title = request.Title,
                Description = request.Description,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                UserLogin = userLogin
            };

            var serviceMock = new Mock<IBookingService>();
            serviceMock
                .Setup(s => s.UpdateBookingAsync(bookingId, request, userLogin))
                .ReturnsAsync(updated);

            var sut = new UpdateBookingCommandHandler(serviceMock.Object);
            var cmd = new UpdateBookingCommand(bookingId, request, userLogin);

            // act
            var result = await sut.Handle(cmd, CancellationToken.None);

            // assert
            Assert.NotNull(result);
            Assert.Equal(updated.Id, result.Id);
            Assert.Equal(updated.RoomId, result.RoomId);
            Assert.Equal(updated.Title, result.Title);
            Assert.Equal(updated.Description, result.Description);
            Assert.Equal(updated.StartTime, result.StartTime);
            Assert.Equal(updated.EndTime, result.EndTime);
            Assert.Equal(updated.UserLogin, result.UserLogin);

            serviceMock.Verify(s => s.UpdateBookingAsync(bookingId, request, userLogin), Times.Once);
        }
    }
}
