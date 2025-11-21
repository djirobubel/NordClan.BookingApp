using NordClan.BookingApp.Shared.Models;
using System.Net.Http.Json;
using System.Web;

namespace NordClan.BookingApp.Client.Services
{
    public class BookingService : IBookingService
    {
        private readonly HttpClient _httpClient;

        public BookingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Booking>> GetBookingsAsync(int? roomId = null, DateTime? date = null)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (roomId.HasValue) query["roomId"] = roomId.ToString();
            if (date.HasValue) query["date"] = date.Value.ToString("yyyy-MM-dd");

            var url = $"bookings?{query}";

            var bookings = await _httpClient.GetFromJsonAsync<List<Booking>>(url);
            return bookings ?? new List<Booking>();
        }

        public async Task<Booking> CreateBookingAsync(BookingRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("bookings", request);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                errorText = errorText.Trim('"').Replace("\\r\\n", "\n");
                throw new HttpRequestException(errorText, null, response.StatusCode);
            }

            return await response.Content.ReadFromJsonAsync<Booking>() ?? new();
        }

        public async Task UpdateBookingAsync(int id, BookingRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"bookings/{id}", request);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                errorText = errorText.Trim('"').Replace("\\r\\n", "\n");
                throw new HttpRequestException(errorText, null, response.StatusCode);
            }
        }

        public async Task DeleteBookingAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"bookings/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                errorText = errorText.Trim('"').Replace("\\r\\n", "\n");
                throw new HttpRequestException(errorText, null, response.StatusCode);
            }
        }
    }
}
