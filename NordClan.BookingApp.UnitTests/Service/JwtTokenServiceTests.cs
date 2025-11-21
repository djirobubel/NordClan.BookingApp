using Microsoft.Extensions.Options;
using NordClan.BookingApp.Api.Options;
using NordClan.BookingApp.Api.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace NordClan.BookingApp.UnitTests.Service
{
    public class JwtTokenServiceTests
    {
        [Fact]
        public void GenerateToken_ReturnsValidJwt_WithExpectedIssuerAudienceAndNameClaim()
        {
            // arrange
            var options = new JwtOptions
            {
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                Key = "SuperSecretKey_ForTests_1234567890",
                ExpiresHours = 1
            };

            var opts = Options.Create(options);
            var sut = new JwtTokenService(opts);
            var username = "test.user";

            // act
            var tokenString = sut.GenerateToken(username);

            // assert
            Assert.False(string.IsNullOrWhiteSpace(tokenString));
            var handler = new JwtSecurityTokenHandler();
            Assert.True(handler.CanReadToken(tokenString));
            var jwt = handler.ReadJwtToken(tokenString);
            Assert.Equal(options.Issuer, jwt.Issuer);
            Assert.Contains(options.Audience, jwt.Audiences);
            var nameClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            Assert.NotNull(nameClaim);
            Assert.Equal(username, nameClaim!.Value);
            Assert.True(jwt.ValidTo > DateTime.UtcNow);
        }

        [Fact]
        public void GenerateToken_SetsExpiration_AccordingToExpiresHours()
        {
            // arrange
            var options = new JwtOptions
            {
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                Key = "AnotherSecretKey_ForTests_0987654321",
                ExpiresHours = 2
            };

            var opts = Options.Create(options);
            var sut = new JwtTokenService(opts);
            var username = "test.user";

            var before = DateTime.UtcNow;

            // act
            var tokenString = sut.GenerateToken(username);

            // assert
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(tokenString);
            var validTo = jwt.ValidTo;
            Assert.True(validTo > before);
            var diff = validTo - before;
            Assert.InRange(diff.TotalHours, options.ExpiresHours - 0.2, options.ExpiresHours + 0.2);
        }
    }
}
