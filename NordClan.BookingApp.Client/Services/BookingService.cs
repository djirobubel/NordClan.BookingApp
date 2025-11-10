using Microsoft.AspNetCore.Components;
using NordClan.BookingApp.Shared.Models;
using System.Diagnostics;  
using System.Net.Http.Json;
using System.Web;

namespace NordClan.BookingApp.Client.Services
{
    public class BookingService : IBookingService
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigation;  

        public BookingService(HttpClient httpClient, NavigationManager navigation)
        {
            _httpClient = httpClient;
            _navigation = navigation;
        }

        private async Task<T> MeasureTimeAsync<T>(Func<Task<T>> action, string successMessage = "Успех!")
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var result = await action();
                stopwatch.Stop();
                UpdateResponseTime($"{stopwatch.ElapsedMilliseconds} мс");
                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                UpdateResponseTime($"Ошибка: {stopwatch.ElapsedMilliseconds} мс");
                throw;
            }
        }

        private void UpdateResponseTime(string time)
        {
            ResponseTimeService.UpdateTime(time);
        }

        public async Task<List<Booking>> GetBookingsAsync(int? roomId = null, DateTime? date = null)
        {
            return await MeasureTimeAsync(async () =>
            {
                var query = HttpUtility.ParseQueryString(string.Empty);
                if (roomId.HasValue) query["roomId"] = roomId.ToString();
                if (date.HasValue) query["date"] = date.Value.ToString("yyyy-MM-dd");

                var url = $"bookings?{query}";
                var bookings = await _httpClient.GetFromJsonAsync<List<Booking>>(url);
                return bookings ?? new List<Booking>();
            });
        }

        //public async Task<Booking> CreateBookingAsync(BookingRequest request)
        //{
        //    return await MeasureTimeAsync(async () =>
        //    {
        //        var response = await _httpClient.PostAsJsonAsync("bookings", request);
        //        response.EnsureSuccessStatusCode();
        //        return await response.Content.ReadFromJsonAsync<Booking>() ?? new();
        //    }, "Сохранено!");
        //}

        public async Task<Booking> CreateBookingAsync(BookingRequest request)
        {
            return await MeasureTimeAsync(async () =>
            {
                var response = await _httpClient.PostAsJsonAsync("bookings", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorText = await response.Content.ReadAsStringAsync();
                    // ←←← УБИРАЕМ КАВЫЧКИ, ЕСЛИ JSON ОТДАЁТ "текст"
                    errorText = errorText.Trim('"').Replace("\\r\\n", "\n");
                    throw new HttpRequestException(errorText, null, response.StatusCode);
                }

                return await response.Content.ReadFromJsonAsync<Booking>() ?? new();
            }, "Сохранено!");
        }

        //public async Task UpdateBookingAsync(int id, BookingRequest request)
        //{
        //    await MeasureTimeAsync(async () =>
        //    {
        //        var response = await _httpClient.PutAsJsonAsync($"bookings/{id}", request);
        //        response.EnsureSuccessStatusCode();
        //        return true;
        //    }, "Обновлено!");
        //}

        public async Task UpdateBookingAsync(int id, BookingRequest request)
        {
            await MeasureTimeAsync(async () =>
            {
                var response = await _httpClient.PutAsJsonAsync($"bookings/{id}", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorText = await response.Content.ReadAsStringAsync();
                    errorText = errorText.Trim('"').Replace("\\r\\n", "\n");
                    throw new HttpRequestException(errorText, null, response.StatusCode);
                }

                return true;
            }, "Обновлено!");
        }

        public async Task DeleteBookingAsync(int id)
        {
            await MeasureTimeAsync(async () =>
            {
                var response = await _httpClient.DeleteAsync($"bookings/{id}");
                response.EnsureSuccessStatusCode();
                return true;
            }, "Удалено!");
        }
    }
}
