namespace NordClan.BookingApp.Api.Exceptions
{
    public class BookingNotFoundException : Exception
    {
        public BookingNotFoundException(string message) : base(message) { }
    }
}
