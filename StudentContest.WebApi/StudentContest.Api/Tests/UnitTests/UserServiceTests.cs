using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Moq;
using StudentContest.Api.Auth;
using StudentContest.Api.Helpers;
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

        public UserServiceTests()
        {
            _context = new DatabaseFake().GetContext();
        }
        
        [Fact]
        public async Task Login_InvalidEmail_ThrowsException()
        {
            var refreshTokenRepoMock = new Mock<IRefreshTokenRepository>().Object;
            var userService = new UserService(_context, new Mock<IRegisterRequestValidator>().Object,
                new Mock<IPasswordHasher>().Object, new Mock<RefreshTokenValidator>(new Mock<AuthenticationConfiguration>().Object).Object,
                refreshTokenRepoMock, new Mock<Authenticator>(new Mock<ITokenGenerator>().Object, refreshTokenRepoMock).Object);
            var loginRequest = new LoginRequest {Email = "new@example.com", Password = "12345678"};

            await Assert.ThrowsAsync<InvalidCredentialException>(() => userService.Login(loginRequest));
        }

        [Fact]
        public async Task Login_InvalidPassword_ThrowsException()
        {
           var bCryptFake = new Mock<IPasswordHasher>();
            bCryptFake.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string password, string hash) => password == hash);
            var refreshTokenRepoMock = new Mock<IRefreshTokenRepository>().Object;
            var userService = new UserService(_context, new Mock<IRegisterRequestValidator>().Object,
                bCryptFake.Object, new Mock<RefreshTokenValidator>(new Mock<AuthenticationConfiguration>().Object).Object,
                refreshTokenRepoMock, new Mock<Authenticator>(new Mock<ITokenGenerator>().Object, refreshTokenRepoMock).Object);
            var loginRequest = new LoginRequest { Email = "test@example.com", Password = "123456" };

            await Assert.ThrowsAsync<InvalidCredentialException>(() => userService.Login(loginRequest));
        }

        [Fact]
        public async Task Login_Success_AddRefreshToken()
        {
            var refreshTokenRepoFake = new Mock<IRefreshTokenRepository>();
            refreshTokenRepoFake.Setup(x => x.Create(It.IsAny<RefreshToken>())).Callback((RefreshToken _) =>
            {
                _context.RefreshTokens.Add(new RefreshToken {Id = -1, Token = "refreshToken", UserId = 1});
                _context.SaveChanges();
            });
            var tokenGeneratorFake = new Mock<ITokenGenerator>();
            tokenGeneratorFake.Setup(x => x.GenerateJwtToken(It.IsAny<User>())).Returns("token");
            tokenGeneratorFake.Setup(x => x.GenerateRefreshToken()).Returns("refreshToken");
            var userService = new UserService(_context, new Mock<IRegisterRequestValidator>().Object,
                new BCryptPasswordHasher(), new Mock<RefreshTokenValidator>(new AuthenticationConfiguration()).Object,
                refreshTokenRepoFake.Object, new Authenticator(tokenGeneratorFake.Object, refreshTokenRepoFake.Object));
            var loginRequest = new LoginRequest {Email = "test@example.com", Password = "12345678"};
            var count = _context.RefreshTokens.Count();

            var result = await userService.Login(loginRequest);

            Assert.IsType<AuthenticatedResponse>(result);
            Assert.Equal("refreshToken", result.RefreshToken);
            Assert.Equal("token", result.Token);
            Assert.Equal(count+1, _context.RefreshTokens.Count());
            Assert.Equal("refreshToken",_context.RefreshTokens.FirstOrDefault(x => x.UserId==1)!.Token);
        }

        [Fact]
        public async Task Logout_Success_ChangesDatabase()
        {
            var refreshTokenRepoFake = new Mock<IRefreshTokenRepository>();
            refreshTokenRepoFake.Setup(x => x.DeleteAll(It.IsAny<int>())).Callback((int id) =>
            {
                var tkn =_context.RefreshTokens.FirstOrDefault(x => x.UserId==id);
                _context.RefreshTokens.Remove(tkn);
                _context.SaveChanges();
            });
            var userService = new UserService(_context, new Mock<IRegisterRequestValidator>().Object,
                new Mock<IPasswordHasher>().Object, new Mock<RefreshTokenValidator>(new AuthenticationConfiguration()).Object,
                refreshTokenRepoFake.Object, new Mock<Authenticator>(new Mock<ITokenGenerator>().Object, new Mock<IRefreshTokenRepository>().Object).Object);
            var count = _context.RefreshTokens.Count();

            await userService.Logout(3);

            Assert.Equal(count - 1, _context.RefreshTokens.Count());
            Assert.Null( _context.RefreshTokens.FirstOrDefault(x => x.UserId == 3));
        }

        [Fact]
        public async Task GetUserInfo_NonExistingUser_ThrowsException()
        {
            var userService = new UserService(_context, new Mock<IRegisterRequestValidator>().Object,
                new Mock<IPasswordHasher>().Object, new Mock<RefreshTokenValidator>(new AuthenticationConfiguration()).Object,
                new Mock<IRefreshTokenRepository>().Object, new Mock<Authenticator>(new Mock<ITokenGenerator>().Object, new Mock<IRefreshTokenRepository>().Object).Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => userService.GetUserInfo("token"));
        }

        [Fact]
        public async Task GetUser_Success_ReturnsUser()
        {
            var refreshTokenRepoFake = new Mock<IRefreshTokenRepository>();
            refreshTokenRepoFake.Setup(x => x.GetByToken(It.IsAny<string>())).ReturnsAsync((string token) =>
                _context.RefreshTokens.FirstOrDefault(t => t.Token == token));
            var userService = new UserService(_context, new Mock<IRegisterRequestValidator>().Object,
                new Mock<IPasswordHasher>().Object, new Mock<RefreshTokenValidator>(new AuthenticationConfiguration()).Object,
                refreshTokenRepoFake.Object, new Mock<Authenticator>(new Mock<ITokenGenerator>().Object, new Mock<IRefreshTokenRepository>().Object).Object);

            var result = await userService.GetUserInfo("3token");

            Assert.NotNull(result);
            Assert.IsType<User>(result);
            Assert.Equal(3, result!.Id);
        }

        [Fact]
        public async Task Register_Success_ChangesDatabase()
        {
            var userService = new UserService(_context, new Mock<IRegisterRequestValidator>().Object,
                new BCryptPasswordHasher(), new Mock<RefreshTokenValidator>(new AuthenticationConfiguration()).Object,
                new Mock<IRefreshTokenRepository>().Object, new Mock<Authenticator>(new Mock<ITokenGenerator>().Object, new Mock<IRefreshTokenRepository>().Object).Object);
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
