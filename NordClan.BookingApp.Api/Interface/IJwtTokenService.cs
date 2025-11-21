namespace NordClan.BookingApp.Api.Interface
{
    public interface IJwtTokenService
    {
        string GenerateToken(string username);
    }
}
