using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Moq;
using StudentContest.Api.Auth;
using StudentContest.Api.Models;
using StudentContest.Api.Services;
using StudentContest.Api.Services.RefreshTokenRepository;
using StudentContest.Api.Validation;
using Xunit;

namespace StudentContest.Api.Tests.UnitTests
{
    public class AuthServiceTests
    {
        private readonly ApplicationContext _context;
        private readonly IUserManagerWrapper _userManager;

        public AuthServiceTests()
        {
            _context = new DatabaseFake().GetContext();
            _userManager = new TestUserManagerWrapper(_context);
        }

        [Fact]
        public async Task Login_InvalidEmail_ThrowsException()
        {
            var refreshTokenRepoMock = new Mock<IRefreshTokenRepository>().Object;
            var authService = new AuthService(refreshTokenRepoMock,
                new RefreshTokenValidator(new AuthenticationConfiguration()),
                new Authenticator(new Mock<ITokenGenerator>().Object, refreshTokenRepoMock), _userManager);
            var loginRequest = new LoginRequest { Email = "new@example.com", Password = "12345678" };

            await Assert.ThrowsAsync<InvalidCredentialException>(() => authService.Login(loginRequest));
        }

        [Fact]
        public async Task Login_InvalidPassword_ThrowsException()
        {
            var refreshTokenRepoMock = new Mock<IRefreshTokenRepository>().Object;
            var authService = new AuthService(refreshTokenRepoMock,
                new RefreshTokenValidator(new AuthenticationConfiguration()),
                new Authenticator(new Mock<ITokenGenerator>().Object, refreshTokenRepoMock), _userManager);
            var loginRequest = new LoginRequest { Email = "test@example.com", Password = "123456" };

            await Assert.ThrowsAsync<InvalidCredentialException>(() => authService.Login(loginRequest));
        }

        [Fact]
        public async Task Login_Success_AddRefreshToken()
        {
            var refreshTokenRepoFake = new Mock<IRefreshTokenRepository>();
            refreshTokenRepoFake.Setup(x => x.Create(It.IsAny<RefreshToken>())).Callback((RefreshToken _) =>
            {
                _context.RefreshTokens.Add(new RefreshToken { Id = -1, Token = "refreshToken", UserId = 1 });
                _context.SaveChangesAsync();
            });
            var tokenGeneratorFake = new Mock<ITokenGenerator>();
            tokenGeneratorFake.Setup(x => x.GenerateJwtToken(It.IsAny<User>(), It.IsAny<IList<string>>())).Returns("token");
            tokenGeneratorFake.Setup(x => x.GenerateRefreshToken()).Returns("refreshToken");
            var authService = new AuthService(refreshTokenRepoFake.Object,
                new RefreshTokenValidator(new AuthenticationConfiguration()),
                new Authenticator(tokenGeneratorFake.Object, refreshTokenRepoFake.Object), _userManager);
            var loginRequest = new LoginRequest { Email = "test@example.com", Password = "12345678" };
            var count = _context.RefreshTokens.Count();

            var result = await authService.Login(loginRequest);

            Assert.IsType<AuthenticatedResponse>(result);
            Assert.Equal("refreshToken", result.RefreshToken);
            Assert.Equal("token", result.Token);
            Assert.Equal(count + 1, _context.RefreshTokens.Count());
            Assert.Equal("refreshToken", _context.RefreshTokens.FirstOrDefault(x => x.UserId == 1)!.Token);
        }
    }
}
