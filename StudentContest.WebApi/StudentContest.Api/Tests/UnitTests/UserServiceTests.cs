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
using StudentContest.Api.Services.UserRepository;
using StudentContest.Api.Validation;
using Xunit;

namespace StudentContest.Api.Tests.UnitTests
{
    public class UserServiceTests
    {
        private readonly AuthenticationContext _context;
        private readonly DatabaseUserRepository _userRepository;

        public UserServiceTests()
        {
            _context = new DatabaseFake().GetContext();
            _userRepository = new DatabaseUserRepository(_context);
        }

        [Fact]
        public async Task Login_InvalidEmail_ThrowsException()
        {
            var refreshTokenRepoMock = new Mock<ITokenRepository>().Object;
            var userService = new UserService(new Mock<IRegisterRequestValidator>().Object,
                new Mock<IPasswordHasher>().Object,
                new Mock<RefreshTokenValidator>(new Mock<AuthenticationConfiguration>().Object).Object,
                refreshTokenRepoMock,
                new Mock<Authenticator>(new Mock<ITokenGenerator>().Object, refreshTokenRepoMock).Object,
                _userRepository);
            var loginRequest = new LoginRequest {Email = "new@example.com", Password = "12345678"};

            await Assert.ThrowsAsync<InvalidCredentialException>(() => userService.Login(loginRequest));
        }

        [Fact]
        public async Task Login_InvalidPassword_ThrowsException()
        {
            var bCryptFake = new Mock<IPasswordHasher>();
            bCryptFake.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string password, string hash) => password == hash);
            var refreshTokenRepoMock = new Mock<ITokenRepository>().Object;
            var userService = new UserService(new Mock<IRegisterRequestValidator>().Object,
                bCryptFake.Object,
                new Mock<RefreshTokenValidator>(new Mock<AuthenticationConfiguration>().Object).Object,
                refreshTokenRepoMock,
                new Mock<Authenticator>(new Mock<ITokenGenerator>().Object, refreshTokenRepoMock).Object,
                _userRepository);
            var loginRequest = new LoginRequest {Email = "test@example.com", Password = "123456"};

            await Assert.ThrowsAsync<InvalidCredentialException>(() => userService.Login(loginRequest));
        }

        [Fact]
        public async Task Login_Success_AddRefreshToken()
        {
            var refreshTokenRepoFake = new Mock<ITokenRepository>();
            refreshTokenRepoFake.Setup(x => x.Create(It.IsAny<UserTokenSet>())).Callback((UserTokenSet _) =>
            {
                _context.Tokens.Add(new UserTokenSet {Id = -1, RefreshToken = "refreshToken", UserId = 1, AccessToken = "accessToken"});
                _context.SaveChanges();
            });
            var tokenGeneratorFake = new Mock<ITokenGenerator>();
            tokenGeneratorFake.Setup(x => x.GenerateJwtToken(It.IsAny<User>())).Returns("token");
            tokenGeneratorFake.Setup(x => x.GenerateRefreshToken()).Returns("refreshToken");
            var userService = new UserService(new Mock<IRegisterRequestValidator>().Object,
                new BCryptPasswordHasher(), new Mock<RefreshTokenValidator>(new AuthenticationConfiguration()).Object,
                refreshTokenRepoFake.Object, new Authenticator(tokenGeneratorFake.Object, refreshTokenRepoFake.Object),
                _userRepository);
            var loginRequest = new LoginRequest {Email = "test@example.com", Password = "12345678"};
            var count = _context.Tokens.Count();

            var result = await userService.Login(loginRequest);

            Assert.IsType<AuthenticatedResponse>(result);
            Assert.Equal("refreshToken", result.RefreshToken);
            Assert.Equal("token", result.Token);
            Assert.Equal(count + 1, _context.Tokens.Count());
            Assert.Equal("refreshToken", _context.Tokens.FirstOrDefault(x => x.UserId == 1)!.RefreshToken);
            Assert.Equal("accessToken", _context.Tokens.FirstOrDefault(x => x.UserId == 1)!.AccessToken);
        }

        [Fact]
        public async Task Logout_Success_ChangesDatabase()
        {
            var refreshTokenRepoFake = new Mock<ITokenRepository>();
            refreshTokenRepoFake.Setup(x => x.DeleteAll(It.IsAny<int>())).Callback((int id) =>
            {
                var tkn = _context.Tokens.FirstOrDefault(x => x.UserId == id);
                _context.Tokens.Remove(tkn);
                _context.SaveChanges();
            });
            var userService = new UserService(new Mock<IRegisterRequestValidator>().Object,
                new Mock<IPasswordHasher>().Object,
                new Mock<RefreshTokenValidator>(new AuthenticationConfiguration()).Object,
                refreshTokenRepoFake.Object,
                new Mock<Authenticator>(new Mock<ITokenGenerator>().Object, new Mock<ITokenRepository>().Object)
                    .Object, _userRepository);
            var count = _context.Tokens.Count();

            await userService.Logout(3);

            Assert.Equal(count - 1, _context.Tokens.Count());
            Assert.Null(_context.Tokens.FirstOrDefault(x => x.UserId == 3));
        }

        [Fact]
        public async Task GetUserInfo_NonExistingUser_ThrowsException()
        {
            var configuration = new AuthenticationConfiguration
            {
                RefreshTokenSecret = "strong-secret-test-key", Issuer = "some-issuer", Audience = "some-audience",
                RefreshTokenExpirationDays = 1
            };
            var token = new TokenGenerator(configuration).GenerateRefreshToken();
            var userService = new UserService(new Mock<IRegisterRequestValidator>().Object,
                new Mock<IPasswordHasher>().Object, new RefreshTokenValidator(configuration),
                new Mock<ITokenRepository>().Object,
                new Mock<Authenticator>(new Mock<ITokenGenerator>().Object, new Mock<ITokenRepository>().Object)
                    .Object, _userRepository);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => userService.GetUserInfo(token));
        }

        [Fact]
        public async Task Register_Success_ChangesDatabase()
        {
            var userService = new UserService(new Mock<IRegisterRequestValidator>().Object,
                new BCryptPasswordHasher(), new Mock<RefreshTokenValidator>(new AuthenticationConfiguration()).Object,
                new Mock<ITokenRepository>().Object,
                new Mock<Authenticator>(new Mock<ITokenGenerator>().Object, new Mock<ITokenRepository>().Object)
                    .Object, _userRepository);
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
