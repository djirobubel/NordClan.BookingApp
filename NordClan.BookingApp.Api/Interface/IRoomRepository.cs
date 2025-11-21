using NordClan.BookingApp.Api.Models;

namespace NordClan.BookingApp.Api.Interface
{
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> GetRoomsAsync();
    }
}
