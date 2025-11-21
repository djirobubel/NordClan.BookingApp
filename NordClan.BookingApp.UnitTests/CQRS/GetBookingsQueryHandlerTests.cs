using Moq;
using NordClan.BookingApp.Api.CQRS.Queries.GetBookings;
using NordClan.BookingApp.Api.Interface;
using NordClan.BookingApp.Api.Models;

namespace NordClan.BookingApp.UnitTests.CQRS
{
    public class GetBookingsQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ReturnsMappedBookings_FromService()
        {
            // arrange
            var roomId = 1;
            var date = new DateTime(2024, 1, 15);

            var bookings = new List<Booking>
            {
                new Booking
                {
                    Id = 10,
                    RoomId = roomId,
                    UserLogin = "user1",
                    Title = "title",
                    Description = "desc",
                    StartTime = date.AddHours(9),
                    EndTime = date.AddHours(10)
                }
            };

            var serviceMock = new Mock<IBookingService>();
            serviceMock
                .Setup(s => s.GetBookingsAsync(roomId, date))
                .ReturnsAsync(bookings);

            var sut = new GetBookingsQueryHandler(serviceMock.Object);
            var query = new GetBookingsQuery(roomId, date);

            // act
            var result = await sut.Handle(query, CancellationToken.None);

            // assert
            var list = result.ToList();
            Assert.Single(list);
            var item = list.First();
            Assert.Equal(10, item.Id);
            Assert.Equal(roomId, item.RoomId);
            Assert.Equal("user1", item.UserLogin);
            Assert.Equal("title", item.Title);
            Assert.Equal("desc", item.Description);
            Assert.Equal(bookings[0].StartTime, item.StartTime);
            Assert.Equal(bookings[0].EndTime, item.EndTime);

            serviceMock.Verify(s => s.GetBookingsAsync(roomId, date), Times.Once);
        }
    }
}
