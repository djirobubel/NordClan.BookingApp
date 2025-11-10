using NordClan.BookingApp.Shared.Models;

namespace NordClan.BookingApp.Client.Services
{
    public interface IRoomService
    {
        Task<List<Room>> GetRoomsAsync();
    }
}
