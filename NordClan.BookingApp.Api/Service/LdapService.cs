using Microsoft.Extensions.Options;
using NordClan.BookingApp.Api.Interface;
using NordClan.BookingApp.Api.Options;
using Novell.Directory.Ldap;

namespace NordClan.BookingApp.Api.Service
{
    public class LdapService : ILdapService
    {
        private readonly LdapOptions _options;

        public LdapService(IOptions<LdapOptions> options)
        {
            _options = options.Value;
        }

        public Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            try
            {
                using var connection = new LdapConnection();
                connection.Connect(_options.Host, _options.Port);

                var userDn = string.Format(_options.UserDnTemplate, username);
                connection.Bind(userDn, password);

                return Task.FromResult(connection.Bound);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }
    }
}
