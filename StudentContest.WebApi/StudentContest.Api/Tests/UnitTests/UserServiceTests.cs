using System;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Moq;
using StudentContest.Api.Authorization;
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
            var bCryptFake = new Mock<IPasswordHasher>();
            bCryptFake.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string password, string hash) => password == hash);
            var authenticatorFake = new Mock<Authenticator>();
            authenticatorFake.Setup(x => x.Authenticate(It.IsAny<User>())).ReturnsAsync(new AuthenticatedResponse(
                It.IsAny<int>(), "refreshToken", "token"));
            var refreshTokenRepoFake = new Mock<IRefreshTokenRepository>();
            refreshTokenRepoFake.Setup(x => x.Create(It.IsAny<RefreshToken>())).Callback((RefreshToken refreshToken) =>
            {
                _context.RefreshTokens.Add(new RefreshToken {Id = -1, Token = "refreshToken", UserId = 1});
            });
            var userService = new UserService(_context, new Mock<IRegisterRequestValidator>().Object,
                bCryptFake.Object, new Mock<RefreshTokenValidator>().Object,
                refreshTokenRepoFake.Object, authenticatorFake.Object);
            var loginRequest = new LoginRequest { Email = "test@example.com", Password = "12345678" };

            var result = await userService.Login(loginRequest);

            Assert.IsType<AuthenticatedResponse>(result);
            Assert.Equal("refreshToken", result.RefreshToken);
            Assert.Equal("token", result.Token);
            bCryptFake.Verify(x => x.VerifyPassword("12345678", "12345678"), Times.Once);
            Assert.Equal("refreshToken", _context.RefreshTokens.FirstOrDefault(x=> x.UserId==1)!.Token);
        }

        //[Fact]
        //public async Task Logout_InvalidToken_ThrowsException()
        //{
        //    var userService = new UserService(_context, new Mock<IRegisterRequestValidator>().Object,
        //        new Mock<IPasswordHasher>().Object, new Mock<RefreshTokenValidator>().Object,
        //        new Mock<IRefreshTokenRepository>().Object, new Mock<Authenticator>().Object);

        //    await Assert.ThrowsAsync<ArgumentException>(() => userService.Logout(It.IsAny<int>()));
        //}

        //[Fact]
        //public async Task Logout_AlreadyRevokedToken_ThrowsException()
        //{
        //    var userService = new UserService(_context, new Mock<IRegisterRequestValidator>().Object,
        //        new Mock<IPasswordHasher>().Object, new Mock<RefreshTokenValidator>().Object,
        //        new Mock<IRefreshTokenRepository>().Object, new Mock<Authenticator>().Object);


        //    await Assert.ThrowsAsync<ArgumentException>(() => userService.Logout("alreadyRevoked"));
        //}

        //[Fact]
        //public async Task Logout_Success_ChangesDatabase()
        //{
        //    var jwtUtilsFake = new Mock<IJwtUtils>();
        //    jwtUtilsFake
        //        .Setup(x => x.RevokeRefreshToken(It.IsAny<RefreshToken>(), It.IsAny<string>(), It.IsAny<string>(),
        //            It.IsAny<string>())).Callback((RefreshToken token, string _, string _, string _) => token.Token = "justRevokedToken");
        //    var userService = new UserService(_context, new Mock<IRegisterRequestValidator>().Object,
        //        new Mock<IPasswordHasher>().Object, new Mock<RefreshTokenValidator>().Object,
        //        new Mock<IRefreshTokenRepository>().Object, new Mock<Authenticator>().Object);


        //    await userService.Logout();

        //    jwtUtilsFake.Verify(x=> x.RevokeRefreshToken(It.IsAny<RefreshToken>(), "ipAddress", It.IsAny<string>(),
        //        It.IsAny<string>()), Times.Once);
        //    Assert.NotNull(_context.Users.FirstOrDefault(x => x.RefreshTokens.LastOrDefault().Token == "justRevokedToken"));
        //}

        //[Fact]
        //public async Task GetUser_NonExistingUser_ThrowsException()
        //{
        //    var userService = new UserService(_context, new Mock<IJwtUtils>().Object,
        //        new Mock<IRegisterRequestValidator>().Object, new Mock<IPasswordHasher>().Object);

        //    await Assert.ThrowsAsync<KeyNotFoundException>(() => userService.GetCurrentUserInfo(-1));
        //}

        //[Fact]
        //public async Task GetUser_Success_ReturnsUser()
        //{
        //    var userService = new UserService(_context, new Mock<IJwtUtils>().Object,
        //        new Mock<IRegisterRequestValidator>().Object, new Mock<IPasswordHasher>().Object);

        //   var result= await userService.GetUserInfo(1);

        //   Assert.NotNull(result);
        //   Assert.IsType<User>(result);
        //   Assert.Equal(1, result.Id);
        //}

        //[Fact]
        //public async Task Register_Success_ChangesDatabase()
        //{
        //    var bCryptFake = new Mock<IPasswordHasher>();
        //    bCryptFake.Setup(x => x.HashPassword(It.IsAny<string>()))
        //        .Returns((string password) => password);
        //    var userService = new UserService(_context, new Mock<IJwtUtils>().Object,
        //        new Mock<IRegisterRequestValidator>().Object, bCryptFake.Object);
        //    var registerRequest = new RegisterRequest
        //    {
        //        Email = "newUser@example.com", FirstName = "New", LastName = "User",
        //        Password = "12345678"
        //    };
        //    var count = _context.Users.Count();

        //    await userService.Register(registerRequest);

        //    Assert.Equal(count+1, _context.Users.Count());
        //    Assert.Equal("newUser@example.com",_context.Users.Last().Email);
        //}

        //[Fact]
        //public async Task RefreshToken_UnknownToken_ThrowsException()
        //{
        //    var userService = new UserService(_context, new Mock<IJwtUtils>().Object,
        //        new Mock<IRegisterRequestValidator>().Object, new Mock<IPasswordHasher>().Object);

        //    await Assert.ThrowsAsync<ArgumentException>(() => userService.RefreshToken("unknown", "ipAddress"));
        //}

        //[Fact]
        //public async Task RefreshToken_NotActiveToken_ThrowsException()
        //{
        //    var userService = new UserService(_context, new Mock<IJwtUtils>().Object,
        //        new Mock<IRegisterRequestValidator>().Object, new Mock<IPasswordHasher>().Object);

        //    await Assert.ThrowsAsync<ArgumentException>(() => userService.RefreshToken("alreadyRevoked", "ipAddress"));
        //}

        //[Fact]
        //public async Task RefreshToken_Success_ChangesDatabase()
        //{
        //    var jwtUtilsFake = new Mock<IJwtUtils>();
        //    jwtUtilsFake
        //        .Setup(x => x.RotateRefreshToken(It.IsAny<RefreshToken>(), It.IsAny<string>()))
        //        .Returns(new RefreshToken {Token = "rotatedToken"});
        //    var userService = new UserService(_context, jwtUtilsFake.Object,
        //        new Mock<IRegisterRequestValidator>().Object, new Mock<IPasswordHasher>().Object);

        //    await userService.RefreshToken("notRevoked");

        //    Assert.Equal("rotatedToken", _context.Users.Last().RefreshTokens.Last().Token);
        //}
    }
}
