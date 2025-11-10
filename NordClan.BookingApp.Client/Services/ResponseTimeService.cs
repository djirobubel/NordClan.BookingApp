namespace NordClan.BookingApp.Client.Services
{
    public static class ResponseTimeService
    {
        public static event Action<string>? OnTimeUpdated;

        public static void UpdateTime(string time)
        {
            OnTimeUpdated?.Invoke(time);
        }
    }
}
