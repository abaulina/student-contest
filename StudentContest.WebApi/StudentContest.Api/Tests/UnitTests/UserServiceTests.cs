using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using StudentContest.Api.Auth;
using StudentContest.Api.Models;
using StudentContest.Api.Services;
using StudentContest.Api.Services.RefreshTokenRepository;
using StudentContest.Api.Validation;
using Xunit;

namespace StudentContest.Api.Tests.UnitTests
{
    public class UserServiceTests
    {
        private readonly AuthenticationContext _context;
        private readonly UserManager<User> _userManager;

        public UserServiceTests()
        {
            _context = new DatabaseFake().GetContext();
            _userManager = TestUserManager.CreateUserManager(_context);
        }

        [Fact]
        public async Task Login_InvalidEmail_ThrowsException()
        {
            var refreshTokenRepoMock = new Mock<IRefreshTokenRepository>().Object;
            var userService = new UserService(new Mock<RefreshTokenValidator>(new Mock<AuthenticationConfiguration>().Object).Object,
                refreshTokenRepoMock,
                new Mock<Authenticator>(new Mock<ITokenGenerator>().Object, refreshTokenRepoMock).Object,
                _userManager, new RegisterRequestValidator());
            var loginRequest = new LoginRequest {Email = "new@example.com", Password = "12345678"};

            await Assert.ThrowsAsync<InvalidCredentialException>(() => userService.Login(loginRequest));
        }

        [Fact]
        public async Task Login_InvalidPassword_ThrowsException()
        {
            var refreshTokenRepoMock = new Mock<IRefreshTokenRepository>().Object;
            var userService = new UserService(new Mock<RefreshTokenValidator>(new Mock<AuthenticationConfiguration>().Object).Object,
                refreshTokenRepoMock,
                new Mock<Authenticator>(new Mock<ITokenGenerator>().Object, refreshTokenRepoMock).Object,
                _userManager, new RegisterRequestValidator());
            var loginRequest = new LoginRequest {Email = "test@example.com", Password = "123456"};

            await Assert.ThrowsAsync<InvalidCredentialException>(() => userService.Login(loginRequest));
        }

        [Fact]
        public async Task Login_Success_AddRefreshToken()
        {
            var refreshTokenRepoFake = new Mock<IRefreshTokenRepository>();
            refreshTokenRepoFake.Setup(x => x.Create(It.IsAny<RefreshToken>())).Callback((RefreshToken _) =>
            {
                _context.RefreshTokens.Add(new RefreshToken {Id = -1, Token = "refreshToken", UserId = "1"});
                _context.SaveChanges();
            });
            var tokenGeneratorFake = new Mock<ITokenGenerator>();
            tokenGeneratorFake.Setup(x => x.GenerateJwtToken(It.IsAny<User>())).Returns("token");
            tokenGeneratorFake.Setup(x => x.GenerateRefreshToken()).Returns("refreshToken");
            var userService = new UserService(new Mock<RefreshTokenValidator>(new AuthenticationConfiguration()).Object,
                refreshTokenRepoFake.Object, new Authenticator(tokenGeneratorFake.Object, refreshTokenRepoFake.Object),
                _userManager, new RegisterRequestValidator());
            var loginRequest = new LoginRequest {Email = "test@example.com", Password = "12345678"};
            var count = _context.RefreshTokens.Count();

            var result = await userService.Login(loginRequest);

            Assert.IsType<AuthenticatedResponse>(result);
            Assert.Equal("refreshToken", result.RefreshToken);
            Assert.Equal("token", result.Token);
            Assert.Equal(count + 1, _context.RefreshTokens.Count());
            Assert.Equal("refreshToken", _context.RefreshTokens.FirstOrDefault(x => x.UserId == "1")!.Token);
        }

        [Fact]
        public async Task GetUserInfo_NonExistingUser_ThrowsException()
        {
            var userService = new UserService(new RefreshTokenValidator(new AuthenticationConfiguration()),
                new Mock<IRefreshTokenRepository>().Object,
                new Mock<Authenticator>(new Mock<ITokenGenerator>().Object, new Mock<IRefreshTokenRepository>().Object)
                    .Object, _userManager, new RegisterRequestValidator());

            await Assert.ThrowsAsync<KeyNotFoundException>(() => userService.GetUserInfo("-1"));
        }

        [Fact]
        public async Task GetUserInfo_Success_CorrectData()
        {
            var userService = new UserService(new RefreshTokenValidator(new AuthenticationConfiguration()),
                new Mock<IRefreshTokenRepository>().Object,
                new Mock<Authenticator>(new Mock<ITokenGenerator>().Object, new Mock<IRefreshTokenRepository>().Object)
                    .Object, _userManager, new RegisterRequestValidator());

            var result = await userService.GetUserInfo("1");

            Assert.Equal("1",result.Id);
            Assert.Equal("test@example.com",result.Email);
        }

        [Fact]
        public async Task Register_Success_ChangesDatabase()
        {
            var userService = new UserService(new Mock<RefreshTokenValidator>(new AuthenticationConfiguration()).Object,
                new Mock<IRefreshTokenRepository>().Object,
                new Mock<Authenticator>(new Mock<ITokenGenerator>().Object, new Mock<IRefreshTokenRepository>().Object)
                    .Object, _userManager, new RegisterRequestValidator());
            var registerRequest = new RegisterRequest
            {
                Email = "newUser@example.com",
                FirstName = "New",
                LastName = "User",
                Password = "12345678"
            };
            var count = _context.Users.Count();

            await userService.Register(registerRequest);

            Assert.Equal(count + 1, _context.Users.Count());
            Assert.Equal("newUser@example.com", _context.Users.Last().Email);
        }
    }
}
