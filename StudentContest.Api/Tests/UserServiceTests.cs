using System;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Moq;
using StudentContest.Api.Authorization;
using StudentContest.Api.Helpers;
using StudentContest.Api.Models;
using StudentContest.Api.Services;
using StudentContest.Api.Validation;
using Xunit;

namespace Tests
{
    public class UserServiceTests
    {
        private UserService _userService;
        private readonly UserContext _context;

        public UserServiceTests()
        {
            _context = new UsersDatabaseFake().GetContext();
        }
        
        [Fact]
        public async Task Login_InvalidEmail_ThrowsException()
        {
            _userService = new UserService(_context, new Mock<IJwtUtils>().Object,
                new Mock<IRegisterRequestValidator>().Object, new Mock<IPasswordHasher>().Object);
            var loginRequest = new LoginRequest {Email = "new@example.com", Password = "12345678"};

            await Assert.ThrowsAsync<InvalidCredentialException>(() => _userService.Login(loginRequest, "ipAddress"));
        }

        [Fact]
        public async Task Login_InvalidPassword_ThrowsException()
        {
           var bCryptFake = new Mock<IPasswordHasher>();
            bCryptFake.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string password, string hash) => password == hash);
            _userService = new UserService(_context, new Mock<IJwtUtils>().Object,
                new Mock<IRegisterRequestValidator>().Object, bCryptFake.Object);
            var loginRequest = new LoginRequest { Email = "test@example.com", Password = "123456" };

            await Assert.ThrowsAsync<InvalidCredentialException>(() => _userService.Login(loginRequest, "ipAddress"));
        }

        [Fact]
        public async Task Login_Success_AddRefreshToken()
        {
            var bCryptFake = new Mock<IPasswordHasher>();
            bCryptFake.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string password, string hash) => password == hash);
            var jwtUtilsFake = new Mock<IJwtUtils>();
            jwtUtilsFake.Setup(x => x.GenerateRefreshToken(It.IsAny<string>())).Returns(new RefreshToken
            {
                Token = "refreshToken"
            });
            jwtUtilsFake.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns("token");
            jwtUtilsFake.Setup(x => x.RemoveOldRefreshTokens(It.IsAny<User>()));
            _userService = new UserService(_context, jwtUtilsFake.Object,
                new Mock<IRegisterRequestValidator>().Object, bCryptFake.Object);
            var loginRequest = new LoginRequest { Email = "test@example.com", Password = "12345678" };

            var result = await _userService.Login(loginRequest, "ipAddress");

            Assert.IsType<AuthenticateResponse>(result);
            Assert.Equal("refreshToken", result.RefreshToken);
            Assert.Equal("token", result.Token);
            jwtUtilsFake.Verify(x=>x.GenerateRefreshToken("ipAddress"), Times.Once);
            bCryptFake.Verify(x=>x.VerifyPassword("12345678","12345678"),Times.Once);
            Assert.Equal("refreshToken", _context.Users.FirstOrDefault(u => u.Email==loginRequest.Email).RefreshTokens.Last().Token);
        }

        [Fact]
        public async Task Logout_InvalidToken_ThrowsException()
        {
            _userService = new UserService(_context, new Mock<IJwtUtils>().Object,
                new Mock<IRegisterRequestValidator>().Object, new Mock<IPasswordHasher>().Object);

            await Assert.ThrowsAsync<ArgumentException>(() => _userService.Logout("", "ipAddress"));
        }

        [Fact]
        public async Task Logout_AlreadyRevokedToken_ThrowsException()
        {
            _userService = new UserService(_context, new Mock<IJwtUtils>().Object,
                new Mock<IRegisterRequestValidator>().Object, new Mock<IPasswordHasher>().Object);

            await Assert.ThrowsAsync<ArgumentException>(() => _userService.Logout("alreadyRevoked", "ipAddress"));
        }

        [Fact]
        public async Task Logout_Success_ChangesDatabase()
        {
            var jwtUtilsFake = new Mock<IJwtUtils>();
            jwtUtilsFake
                .Setup(x => x.RevokeRefreshToken(It.IsAny<RefreshToken>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>())).Callback((RefreshToken token, string _, string _, string _) => token.Token = "justRevokedToken");
            _userService = new UserService(_context, jwtUtilsFake.Object,
                new Mock<IRegisterRequestValidator>().Object, new Mock<IPasswordHasher>().Object);

            await _userService.Logout("notRevoked", "ipAddress");

            jwtUtilsFake.Verify(x=> x.RevokeRefreshToken(It.IsAny<RefreshToken>(), "ipAddress", It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
            Assert.NotNull(_context.Users.FirstOrDefault(x => x.RefreshTokens.LastOrDefault().Token == "justRevokedToken"));
        }
    }
}
