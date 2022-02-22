using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using StudentContest.Api.Models;
using StudentContest.Api.Tests.Helpers;
using Xunit;

namespace StudentContest.Api.Tests.IntegrationTests
{
    public class StudentContestApiTests : IClassFixture<ApiWebAppFactory<Program>>
    {
        private readonly HttpClient _client;

        public StudentContestApiTests(ApiWebAppFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetUserInfo_UnauthorizedAccess_NotAllowed()
        {
            var response = await _client.GetAsync("users/0");

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task RegisterUser_Success_ReturnsOk()
        {
            var registerRequest = new RegisterRequest
                {Email = "notUsed@example.com", FirstName = "Test", LastName = "User", Password = "12345678"};
            var loginRequest = new LoginRequest{ Email = registerRequest.Email, Password = registerRequest.Password };
            _client.DefaultRequestHeaders.Add("X-Forwarded-For", "ipAddress");

            var response = await _client.PostAsync("users/register", Utilities.GetStringContent(registerRequest));
            var loginResponse = await _client.PostAsync("users/login", Utilities.GetStringContent(loginRequest));

            response.EnsureSuccessStatusCode();
            loginResponse.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("test@example.com", "Test", "User", "12345678")]
        [InlineData("test@.com", "Test", "User", "12345678")]
        [InlineData("test@example.com", ".Test", "User", "12345678")]
        [InlineData("test@example.com", "Test", "Use.r.", "12345678")]
        [InlineData("test@example.com", "Test", "User", "12")]
        [InlineData("", "", "", "")]
        public async Task RegisterUser_InvalidData_ReturnsBadRequest(string email, string firstName, string lastName, string password)
        {
            var registerRequest = new RegisterRequest
                { Email = email, FirstName = firstName, LastName = lastName, Password = password };

            var response = await _client.PostAsync("users/register", Utilities.GetStringContent(registerRequest));

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RegisterUser_IncorrectBodyType_ReturnsBadRequest()
        {
            var registerRequest = new {Email = "new@example.com", Password = "12345678", Username = "user"};

            var response = await _client.PostAsync("users/register", Utilities.GetStringContent(registerRequest));

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_IncorrectBodyType_ReturnsBadRequest()
        {
            var loginRequest = new { Email = "new@example.com", Password = "12345678", Username = "user" };

            var response = await _client.PostAsync("users/login", Utilities.GetStringContent(loginRequest));

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("t@example.com", "12345678")]
        [InlineData("test@example.com", "123456")]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized(string email, string password)
        {
            var loginRequest = new LoginRequest
                { Email = email, Password = password };
            _client.DefaultRequestHeaders.Add("X-Forwarded-For", "ipAddress");

            var response = await _client.PostAsync("users/login", Utilities.GetStringContent(loginRequest));

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_Success_ReturnsOkAndValidTokens()
        {
            var loginRequest = new LoginRequest
            { Email = "test@example.com", Password = "12345678" };
            _client.DefaultRequestHeaders.Add("X-Forwarded-For", "ipAddress");

            var response = await _client.PostAsync("users/login", Utilities.GetStringContent(loginRequest));
            var result = await response.Content.ReadAsAsync<AuthenticatedResponse>();
            _client.DefaultRequestHeaders.Add("Authorization",result.Token);
            var getResponse = await _client.GetAsync($"users/{result.Id}");
            var getResponseResult = await getResponse.Content.ReadAsAsync<User>();

            response.EnsureSuccessStatusCode();
            getResponse.EnsureSuccessStatusCode();
            Assert.Equal(loginRequest.Email, getResponseResult.Email);
        }

        [Fact]
        public async Task Logout_NoTokenInCookies_ReturnsBadRequest()
        {
            var response = await _client.DeleteAsync("users/logout");
            
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Logout_InvalidToken_ReturnsBadRequest()
        {
            _client.DefaultRequestHeaders.Add("X-Forwarded-For", "ipAddress");
            _client.DefaultRequestHeaders.Add(HeaderNames.Cookie, "refreshToken=");
            
            var response = await _client.DeleteAsync("users/logout");

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Logout_Success_ImpossibleToReuseToken()
        {
            _client.DefaultRequestHeaders.Add("X-Forwarded-For", "ipAddress");
            _client.DefaultRequestHeaders.Add(HeaderNames.Cookie, "refreshToken=notRevoked");

            var response = await _client.DeleteAsync("users/logout");
            var getResponse = await _client.GetAsync("users/4");

            response.EnsureSuccessStatusCode();
            Assert.False(getResponse.IsSuccessStatusCode);
        }

        [Fact]
        public async Task RefreshToken_NoTokenInCookies_ReturnsBadRequest()
        {
            var response = await _client.PostAsync("users/refresh-token", null);

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RefreshToken_InvalidToken_ReturnsBadRequest()
        {
            _client.DefaultRequestHeaders.Add("X-Forwarded-For", "ipAddress");
            _client.DefaultRequestHeaders.Add(HeaderNames.Cookie, "refreshToken=");

            var response = await _client.PostAsync("users/refresh-token", null);
            
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RefreshToken_Success_NewTokenIsValid()
        {
            _client.DefaultRequestHeaders.Add("X-Forwarded-For", "ipAddress");
            _client.DefaultRequestHeaders.Add(HeaderNames.Cookie, "refreshToken=notRevoked");

            var response = await _client.PostAsync("users/refresh-token",null);
            var result = await response.Content.ReadAsAsync<AuthenticatedResponse>();
            _client.DefaultRequestHeaders.Add(HeaderNames.Cookie, "refreshToken=" + result.RefreshToken);
            var getResponse = await _client.GetAsync($"users/4");

            response.EnsureSuccessStatusCode();
            getResponse.EnsureSuccessStatusCode();
        }
    }
}
