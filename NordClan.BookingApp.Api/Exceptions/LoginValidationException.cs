namespace NordClan.BookingApp.Api.Exceptions
{
    public class LoginValidationException : Exception
    {
        public LoginValidationException(string message) : base(message) { }
    }
}
