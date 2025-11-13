using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NordClan.BookingApp.Api.Models;
using Novell.Directory.Ldap;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NordClan.BookingApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private const string LdapHost = "ldap";
        private const int LdapPort = 389;
        private const string LdapBaseDn = "dc=nordclan,dc=local";
        private const string LdapUserDnTemplate = "cn={0},ou=Users,dc=nordclan,dc=local";
        private readonly IConfiguration _config;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public IActionResult Post([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Логин и пароль обязательны!");

            bool isValid = ValidateLdapUser(request.Username, request.Password);

            if (!isValid)
                return Unauthorized("Неверный логин или пароль!");

            var token = GenerateJwtToken(request.Username);

            return Ok(new LoginResponse
            {
                Token = token,
                Username = request.Username
            });
        }

        private bool ValidateLdapUser(string username, string password)
        {
            try
            {
                using var connection = new LdapConnection();
                connection.Connect(LdapHost, LdapPort);
                var temp = string.Format(LdapUserDnTemplate, username);
                connection.Bind(string.Format(LdapUserDnTemplate, username), password);
                return connection.Bound;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateJwtToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],  
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
