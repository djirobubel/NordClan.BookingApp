using Moq;
using NordClan.BookingApp.Api.CQRS.Queries.GetRooms;
using NordClan.BookingApp.Api.Interface;
using NordClan.BookingApp.Api.Models;

namespace NordClan.BookingApp.UnitTests.CQRS
{
    public class GetRoomsQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ReturnsMappedRooms_FromRepository()
        {
            // arrange
            var rooms = new List<Room>
            {
                new Room { Id = 1, Name = "Меркурий", Colour = "#111111" },
                new Room { Id = 2, Name = "Венера",   Colour = "#222222" }
            };

            var repoMock = new Mock<IRoomRepository>();
            repoMock
                .Setup(r => r.GetRoomsAsync())
                .ReturnsAsync(rooms);

            var sut = new GetRoomsQueryHandler(repoMock.Object);
            var query = new GetRoomsQuery();

            // act
            var result = await sut.Handle(query, CancellationToken.None);

            // assert
            var list = result.ToList();
            Assert.Equal(2, list.Count);
            Assert.Contains(list, r => r.Id == 1 && r.Name == "Меркурий" && r.Colour == "#111111");
            Assert.Contains(list, r => r.Id == 2 && r.Name == "Венера" && r.Colour == "#222222");

            repoMock.Verify(r => r.GetRoomsAsync(), Times.Once);
        }
    }
}
