namespace NordClan.BookingApp.Api.Options
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public string Key { get; set; } = null!;
        public int ExpiresHours { get; set; } = 8;
    }
}
