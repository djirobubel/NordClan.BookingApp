namespace NordClan.BookingApp.Api.Options
{
    public class LdapOptions
    {
        public string Host { get; set; } = null!;
        public int Port { get; set; } = 389;
        public string UserDnTemplate { get; set; } = null!;
        public string BaseDn { get; set; } = null!;
    }
}
