namespace NordClan.BookingApp.Client.Services;
public interface IAuthService
{
    Task<bool> LoginAsync(string username, string password);
    void Logout();
}
