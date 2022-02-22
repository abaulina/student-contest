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

namespace StudentContest.Api.Tests.UnitTests
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
            _userServiceFake.Setup(x => x.GetUserInfo(It.IsAny<int>())).ReturnsAsync(user);
            var controller = new UsersController(_userServiceFake.Object);

            var result = await controller.GetUser();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<User>(okResult.Value);
            Assert.Equal("test@example.com", returnValue.Email);
        }

        [Fact]
        public async Task Logout_Success_ReturnsOk()
        {
            var httpContext = new DefaultHttpContext
            {
                Request =
                {
                    Cookies = MockRequestCookieCollection("refreshToken", "refreshToken")
                }
            };

            _userServiceFake.Setup(x => x.Logout(It.IsAny<int>()));
            var controller = new UsersController(_userServiceFake.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext,
                }
            };

            var result = await controller.Logout();

            Assert.IsType<OkResult>(result);
            _userServiceFake.Verify(x => x.Logout(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task Login_Success_ReturnsOkWithData()
        {
            var httpContext = new DefaultHttpContext();
            var user = new User
                { Email = "test@example.com", FirstName = "Test", LastName = "User", PasswordHash = "12345678" };
            _userServiceFake.Setup(x => x.Login(It.IsAny<LoginRequest>())).ReturnsAsync(new AuthenticatedResponse(user.Id, "token", "refreshToken"));
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
            var returnValue = Assert.IsType<AuthenticatedResponse>(okResult.Value);
            Assert.Equal(user.Id,returnValue.Id);
            Assert.Equal("token",returnValue.Token);
            Assert.Equal("refreshToken", returnValue.RefreshToken);
        }

        [Fact]
        public async Task RefreshToken_Success_ReturnsOkWithData()
        {
            var httpContext = new DefaultHttpContext
            {
                Request =
                {
                    Cookies = MockRequestCookieCollection("refreshToken", "refreshToken")
                }
            };

            var user = new User
                { Email = "test@example.com", FirstName = "Test", LastName = "User", PasswordHash = "12345678" };
            _userServiceFake.Setup(x => x.RefreshToken(It.IsAny<string>())).ReturnsAsync(new AuthenticatedResponse(user.Id, "token", "refreshToken"));
            var controller = new UsersController(_userServiceFake.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            var result = await controller.RefreshToken();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<AuthenticatedResponse>(okResult.Value);
            Assert.Equal(user.Id, returnValue.Id);
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
    }
}