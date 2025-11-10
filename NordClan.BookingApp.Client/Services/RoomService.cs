using NordClan.BookingApp.Shared.Models;
using System.Net.Http.Json;

namespace NordClan.BookingApp.Client.Services
{
    public class RoomService : IRoomService
    {
        private readonly HttpClient _httpClient;

        public RoomService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Room>> GetRoomsAsync()
        {
            var rooms = await _httpClient.GetFromJsonAsync<List<Room>>("Rooms");

            return rooms ?? new List<Room>();
        }
    }
}
