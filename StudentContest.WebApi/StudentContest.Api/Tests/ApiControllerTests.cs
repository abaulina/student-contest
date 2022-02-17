using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Moq;
using StudentContest.Api.Controllers;
using StudentContest.Api.Models;
using StudentContest.Api.Services;
using Xunit;

namespace Tests
{
    public class ApiControllerTests
    {
        private readonly Mock<IUserService> _userServiceFake;

        public ApiControllerTests()
        {
            _userServiceFake = new Mock<IUserService>();
        }
        
        [Fact]
        public async Task RegisterUser_Success_ReturnsOk()
        {
            _userServiceFake.Setup(x => x.Register(It.IsAny<RegisterRequest>()));
            var controller = new UsersController(_userServiceFake.Object);
            var registerRequest = new RegisterRequest
                {Email = "newUser@example.com", FirstName = "Test", LastName = "User", Password = "12345678"};

            var result = await controller.RegisterUser(registerRequest);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GetUser_Success_ReturnsOkWithData()
        {
            var user = new User
                {Email = "test@example.com", FirstName = "Test", LastName = "User", PasswordHash = "12345678"};
            _userServiceFake.Setup(x => x.GetCurrentUserInfo(It.IsAny<int>())).ReturnsAsync(user);
            var controller = new UsersController(_userServiceFake.Object);

            var result = await controller.GetUser(0);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<User>(okResult.Value);
            Assert.Equal("test@example.com", returnValue.Email);
        }

        [Fact]
        public async Task Logout_Success_ReturnsOk()
        {
            var httpContext = CreateHttpContextWithMockRequestIp();
            httpContext.Request.Cookies = MockRequestCookieCollection("refreshToken", "refreshToken");
            
            _userServiceFake.Setup(x => x.Logout(It.IsAny<string>(), It.IsAny<string>()));
            var controller = new UsersController(_userServiceFake.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext,
                }
            };

            var result = await controller.Logout();

            Assert.IsType<OkResult>(result);
            _userServiceFake.Verify(x => x.Logout("refreshToken", "someIp"), Times.Once);
        }

        [Fact]
        public async Task Login_Success_ReturnsOkWithData()
        {
            var httpContext = CreateHttpContextWithMockRequestIp();
            var user = new User
                { Email = "test@example.com", FirstName = "Test", LastName = "User", PasswordHash = "12345678" };
            _userServiceFake.Setup(x => x.Login(It.IsAny<LoginRequest>(), It.IsAny<string>())).ReturnsAsync(new AuthenticateResponse(user, "token", "refreshToken"));
            var loginRequest = new LoginRequest { Email = "test@example.com", Password = "12345678" };
            var controller = new UsersController(_userServiceFake.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            var result = await controller.Login(loginRequest);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<AuthenticateResponse>(okResult.Value);
            Assert.Equal(user.Email,returnValue.Email);
            Assert.Equal("token",returnValue.Token);
            Assert.Equal("refreshToken", returnValue.RefreshToken);
        }

        [Fact]
        public async Task RefreshToken_Success_ReturnsOkWithData()
        {
            var httpContext = CreateHttpContextWithMockRequestIp();
            httpContext.Request.Cookies = MockRequestCookieCollection("refreshToken", "refreshToken");

            var user = new User
                { Email = "test@example.com", FirstName = "Test", LastName = "User", PasswordHash = "12345678" };
            _userServiceFake.Setup(x => x.RefreshToken(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new AuthenticateResponse(user, "token", "refreshToken"));
            var controller = new UsersController(_userServiceFake.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            var result = await controller.RefreshToken();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<AuthenticateResponse>(okResult.Value);
            Assert.Equal(user.Email, returnValue.Email);
            Assert.Equal("token", returnValue.Token);
            Assert.Equal("refreshToken", returnValue.RefreshToken);
        }

        private static IRequestCookieCollection MockRequestCookieCollection(string key, string value)
        {
            var requestFeature = new HttpRequestFeature();
            var featureCollection = new FeatureCollection();

            requestFeature.Headers = new HeaderDictionary();
            requestFeature.Headers.Add(HeaderNames.Cookie, new StringValues(key + "=" + value));

            featureCollection.Set<IHttpRequestFeature>(requestFeature);

            var cookiesFeature = new RequestCookiesFeature(featureCollection);

            return cookiesFeature.Cookies;
        }

        private static DefaultHttpContext CreateHttpContextWithMockRequestIp()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-Forwarded-For"] = "someIp";
            return httpContext;
        }
    }
}