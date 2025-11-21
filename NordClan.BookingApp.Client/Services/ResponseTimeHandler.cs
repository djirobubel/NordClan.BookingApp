using System.Diagnostics;

namespace NordClan.BookingApp.Client.Services
{
    public class ResponseTimeHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var response = await base.SendAsync(request, cancellationToken);
                stopwatch.Stop();
                ResponseTimeService.UpdateTime($"{stopwatch.ElapsedMilliseconds} мс");

                return response;
            }
            catch
            {
                stopwatch.Stop();
                ResponseTimeService.UpdateTime($"Ошибка: {stopwatch.ElapsedMilliseconds} мс");

                throw;
            }
        }
    }
}
