namespace NordClan.BookingApp.Api.Exceptions
{
    public class BookingValidationException : Exception
    {
        public BookingValidationException(string message) : base(message) { }
    }
}
