namespace NordClan.BookingApp.Api.Exceptions
{
    public class BookingAccessDeniedException : Exception
    {
        public BookingAccessDeniedException(string message) : base(message) { }
    }
}
