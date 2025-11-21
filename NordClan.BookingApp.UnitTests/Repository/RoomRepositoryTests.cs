using NordClan.BookingApp.Api.Models;
using NordClan.BookingApp.Api.Repository;

namespace NordClan.BookingApp.UnitTests.Repository
{
    public class RoomRepositoryTests
    {
        [Fact]
        public async Task GetRoomsAsync_ReturnsAllRooms()
        {
            // arrange
            using var context = TestDbContextFactory.CreateDbContext();

            context.Rooms.AddRange(
                new Room { Id = 1, Name = "Меркурий", Colour = "#111111" },
                new Room { Id = 2, Name = "Венера", Colour = "#222222" }
            );
            await context.SaveChangesAsync();

            var sut = new RoomRepository(context);

            // act
            var result = await sut.GetRoomsAsync();

            // assert
            var list = result.ToList();
            Assert.Equal(2, list.Count);
            Assert.Contains(list, r => r.Id == 1 && r.Name == "Меркурий");
            Assert.Contains(list, r => r.Id == 2 && r.Name == "Венера");
        }
    }
}
