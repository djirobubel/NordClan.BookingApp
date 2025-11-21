namespace NordClan.BookingApp.Api.Interface
{
    public interface ILdapService
    {
        Task<bool> ValidateCredentialsAsync(string username, string password);
    }
}
