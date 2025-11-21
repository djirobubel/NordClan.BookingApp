using Moq;
using NordClan.BookingApp.Api.CQRS.Commands.Login;
using NordClan.BookingApp.Api.Interface;

namespace NordClan.BookingApp.UnitTests.CQRS
{
    public class LoginCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ReturnsToken_WhenCredentialsAreValid()
        {
            // arrange
            var username = "user1";
            var password = "password";
            var token = "jwt-token";

            var ldapMock = new Mock<ILdapService>();
            ldapMock
                .Setup(l => l.ValidateCredentialsAsync(username, password))
                .ReturnsAsync(true);

            var jwtMock = new Mock<IJwtTokenService>();
            jwtMock
                .Setup(j => j.GenerateToken(username))
                .Returns(token);

            var sut = new LoginCommandHandler(ldapMock.Object, jwtMock.Object);
            var cmd = new LoginCommand(username, password);

            // act
            var result = await sut.Handle(cmd, CancellationToken.None);

            // assert
            Assert.NotNull(result);
            Assert.Equal(username, result.Username);
            Assert.Equal(token, result.Token);

            ldapMock.Verify(l => l.ValidateCredentialsAsync(username, password), Times.Once);
            jwtMock.Verify(j => j.GenerateToken(username), Times.Once);
        }
    }
}
