using System;
using System.Collections.Generic;
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

namespace StudentContest.Api.Tests
{
    public class UserServiceTests
    {
        private readonly UserContext _context;

        public UserServiceTests()
        {
            _context = new UsersDatabaseFake().GetContext();
        }

        [Fact]
        public async Task Login_InvalidEmail_ThrowsException()
        {
            var userService = new UserService(_context, new Mock<IJwtUtils>().Object,
                new Mock<IRegisterRequestValidator>().Object, new Mock<IPasswordHasher>().Object);
            var loginRequest = new LoginRequest { Email = "new@example.com", Password = "12345678" };

            await Assert.ThrowsAsync<InvalidCredentialException>(() => userService.Login(loginRequest, "ipAddress"));
        }

        [Fact]
        public async Task Login_InvalidPassword_ThrowsException()
        {
            var bCryptFake = new Mock<IPasswordHasher>();
            bCryptFake.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string password, string hash) => password == hash);
            var userService = new UserService(_context, new Mock<IJwtUtils>().Object,
                new Mock<IRegisterRequestValidator>().Object, bCryptFake.Object);
            var loginRequest = new LoginRequest { Email = "test@example.com", Password = "123456" };

            await Assert.ThrowsAsync<InvalidCredentialException>(() => userService.Login(loginRequest, "ipAddress"));
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
            var userService = new UserService(_context, jwtUtilsFake.Object,
                new Mock<IRegisterRequestValidator>().Object, bCryptFake.Object);
            var loginRequest = new LoginRequest { Email = "test@example.com", Password = "12345678" };

            var result = await userService.Login(loginRequest, "ipAddress");

            Assert.IsType<AuthenticateResponse>(result);
            Assert.Equal("refreshToken", result.RefreshToken);
            Assert.Equal("token", result.Token);
            jwtUtilsFake.Verify(x => x.GenerateRefreshToken("ipAddress"), Times.Once);
            bCryptFake.Verify(x => x.VerifyPassword("12345678", "12345678"), Times.Once);
            Assert.Equal("refreshToken", _context.Users.FirstOrDefault(u => u.Email == loginRequest.Email).RefreshTokens.Last().Token);
        }

        [Fact]
        public async Task Logout_InvalidToken_ThrowsException()
        {
            var userService = new UserService(_context, new Mock<IJwtUtils>().Object,
                new Mock<IRegisterRequestValidator>().Object, new Mock<IPasswordHasher>().Object);

            await Assert.ThrowsAsync<ArgumentException>(() => userService.Logout("", "ipAddress"));
        }

        [Fact]
        public async Task Logout_AlreadyRevokedToken_ThrowsException()
        {
            var userService = new UserService(_context, new Mock<IJwtUtils>().Object,
                new Mock<IRegisterRequestValidator>().Object, new Mock<IPasswordHasher>().Object);

            await Assert.ThrowsAsync<ArgumentException>(() => userService.Logout("alreadyRevoked", "ipAddress"));
        }

        [Fact]
        public async Task Logout_Success_ChangesDatabase()
        {
            var jwtUtilsFake = new Mock<IJwtUtils>();
            jwtUtilsFake
                .Setup(x => x.RevokeRefreshToken(It.IsAny<RefreshToken>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>())).Callback((RefreshToken token, string _, string _, string _) => token.Token = "justRevokedToken");
            var userService = new UserService(_context, jwtUtilsFake.Object,
                new Mock<IRegisterRequestValidator>().Object, new Mock<IPasswordHasher>().Object);

            await userService.Logout("notRevoked", "ipAddress");

            jwtUtilsFake.Verify(x => x.RevokeRefreshToken(It.IsAny<RefreshToken>(), "ipAddress", It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
            Assert.NotNull(_context.Users.FirstOrDefault(x => x.RefreshTokens.LastOrDefault().Token == "justRevokedToken"));
        }

        [Fact]
        public async Task GetUser_NonExistingUser_ThrowsException()
        {
            var userService = new UserService(_context, new Mock<IJwtUtils>().Object,
                new Mock<IRegisterRequestValidator>().Object, new Mock<IPasswordHasher>().Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => userService.GetCurrentUserInfo(-1));
        }

        [Fact]
        public async Task GetUser_Success_ReturnsUser()
        {
            var userService = new UserService(_context, new Mock<IJwtUtils>().Object,
                new Mock<IRegisterRequestValidator>().Object, new Mock<IPasswordHasher>().Object);

            var result = await userService.GetCurrentUserInfo(1);

            Assert.NotNull(result);
            Assert.IsType<User>(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task Register_Success_ChangesDatabase()
        {
            var bCryptFake = new Mock<IPasswordHasher>();
            bCryptFake.Setup(x => x.HashPassword(It.IsAny<string>()))
                .Returns((string password) => password);
            var userService = new UserService(_context, new Mock<IJwtUtils>().Object,
                new Mock<IRegisterRequestValidator>().Object, bCryptFake.Object);
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

        [Fact]
        public async Task RefreshToken_UnknownToken_ThrowsException()
        {
            var userService = new UserService(_context, new Mock<IJwtUtils>().Object,
                new Mock<IRegisterRequestValidator>().Object, new Mock<IPasswordHasher>().Object);

            await Assert.ThrowsAsync<ArgumentException>(() => userService.RefreshToken("unknown", "ipAddress"));
        }

        [Fact]
        public async Task RefreshToken_NotActiveToken_ThrowsException()
        {
            var userService = new UserService(_context, new Mock<IJwtUtils>().Object,
                new Mock<IRegisterRequestValidator>().Object, new Mock<IPasswordHasher>().Object);

            await Assert.ThrowsAsync<ArgumentException>(() => userService.RefreshToken("alreadyRevoked", "ipAddress"));
        }

        [Fact]
        public async Task RefreshToken_Success_ChangesDatabase()
        {
            var jwtUtilsFake = new Mock<IJwtUtils>();
            jwtUtilsFake
                .Setup(x => x.RotateRefreshToken(It.IsAny<RefreshToken>(), It.IsAny<string>()))
                .Returns(new RefreshToken { Token = "rotatedToken" });
            var userService = new UserService(_context, jwtUtilsFake.Object,
                new Mock<IRegisterRequestValidator>().Object, new Mock<IPasswordHasher>().Object);

            await userService.RefreshToken("notRevoked", "ipAddress");

            Assert.Equal("rotatedToken", _context.Users.Last().RefreshTokens.Last().Token);
        }
    }
}
