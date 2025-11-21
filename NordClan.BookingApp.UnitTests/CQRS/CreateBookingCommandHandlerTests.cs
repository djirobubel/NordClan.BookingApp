using Moq;
using NordClan.BookingApp.Api.CQRS.Commands.CreateBooking;
using NordClan.BookingApp.Api.Interface;
using NordClan.BookingApp.Api.Models;

namespace NordClan.BookingApp.UnitTests.CQRS
{
    public class CreateBookingCommandHandlerTests
    {
        [Fact]
        public async Task Handle_CallsService_AndReturnsMappedResult()
        {
            // arrange
            var request = new BookingRequest
            {
                RoomId = 1,
                Title = "title",
                Description = "desc",
                StartTime = new DateTime(2024, 1, 15, 9, 0, 0),
                EndTime = new DateTime(2024, 1, 15, 10, 0, 0)
            };
            var userLogin = "user1";

            var created = new Booking
            {
                Id = 10,
                RoomId = request.RoomId,
                Title = request.Title,
                Description = request.Description,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                UserLogin = userLogin
            };

            var serviceMock = new Mock<IBookingService>();
            serviceMock
                .Setup(s => s.CreateBookingAsync(request, userLogin))
                .ReturnsAsync(created);

            var sut = new CreateBookingCommandHandler(serviceMock.Object);
            var cmd = new CreateBookingCommand(userLogin, request);

            // act
            var result = await sut.Handle(cmd, CancellationToken.None);

            // assert
            Assert.NotNull(result);
            Assert.Equal(created.Id, result.Id);
            Assert.Equal(created.RoomId, result.RoomId);
            Assert.Equal(created.Title, result.Title);
            Assert.Equal(created.Description, result.Description);
            Assert.Equal(created.StartTime, result.StartTime);
            Assert.Equal(created.EndTime, result.EndTime);
            Assert.Equal(created.UserLogin, result.UserLogin);

            serviceMock.Verify(s => s.CreateBookingAsync(request, userLogin), Times.Once);
        }
    }
}
