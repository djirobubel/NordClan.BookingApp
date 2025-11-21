namespace NordClan.BookingApp.Api.Exceptions
{
    public class BookingOverlapException : Exception
    {
        public BookingOverlapException(string message) : base(message) { }
    }
}
