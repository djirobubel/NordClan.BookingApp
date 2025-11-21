using MediatR;
using Moq;
using NordClan.BookingApp.Api.CQRS.Commands.DeleteBooking;
using NordClan.BookingApp.Api.Interface;

namespace NordClan.BookingApp.UnitTests.CQRS
{
    public class DeleteBookingCommandHandlerTests
    {
        [Fact]
        public async Task Handle_CallsServiceDelete_AndReturnsUnit()
        {
            // arrange
            var bookingId = 42;
            var userLogin = "user1";

            var serviceMock = new Mock<IBookingService>();
            serviceMock
                .Setup(s => s.DeleteBookingAsync(bookingId, userLogin))
                .Returns(Task.CompletedTask);

            var sut = new DeleteBookingCommandHandler(serviceMock.Object);
            var cmd = new DeleteBookingCommand(bookingId, userLogin);

            // act
            var result = await sut.Handle(cmd, CancellationToken.None);

            // assert
            Assert.Equal(Unit.Value, result);
            serviceMock.Verify(s => s.DeleteBookingAsync(bookingId, userLogin), Times.Once);
        }
    }
}
