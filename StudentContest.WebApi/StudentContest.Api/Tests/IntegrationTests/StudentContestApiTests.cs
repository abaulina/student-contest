using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
            var response = await _client.GetAsync("users");

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task RegisterUser_Success_ReturnsOk()
        {
            var registerRequest = new RegisterRequest
                {Email = "notUsed@example.com", FirstName = "Test", LastName = "User", Password = "12345678"};
            var loginRequest = new LoginRequest{ Email = registerRequest.Email, Password = registerRequest.Password };

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
            var loginRequest = new { Password = "12345678", Username = "user" };

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

            var response = await _client.PostAsync("users/login", Utilities.GetStringContent(loginRequest));
            var result = await response.Content.ReadAsAsync<AuthenticatedResponse>();
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer "+result.Token);
            var getResponse = await _client.GetAsync("users");
            var getResponseResult = await getResponse.Content.ReadAsAsync<User>();

            response.EnsureSuccessStatusCode();
            getResponse.EnsureSuccessStatusCode();
            Assert.Equal(loginRequest.Email, getResponseResult.Email);
        }

        [Fact]
        public async Task Logout_InvalidToken_ReturnsUnauthorized()
        {
            var response = await _client.DeleteAsync("users/logout");

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Logout_Success_ImpossibleToReuseToken()
        {
            var loginRequest = new LoginRequest
                { Email = "test@example.com", Password = "12345678" };
            var loginResponse = await _client.PostAsync("users/login", Utilities.GetStringContent(loginRequest));
            var result = await loginResponse.Content.ReadAsAsync<AuthenticatedResponse>();
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + result.Token);

            var logoutResponse = await _client.DeleteAsync("users/logout");
            var getResponse = await _client.GetAsync("users");

            logoutResponse.EnsureSuccessStatusCode();
            Assert.False(getResponse.IsSuccessStatusCode);
        }

        [Fact]
        public async Task RefreshToken_InvalidToken_ReturnsBadRequest()
        {
            var response = await _client.PostAsync("users/refresh-token", null);

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        [Fact]
        public async Task RefreshToken_Success_NewTokenIsValid()
        {
            var loginRequest = new LoginRequest
                { Email = "test@example.com", Password = "12345678" };
            var loginResponse = await _client.PostAsync("users/login", Utilities.GetStringContent(loginRequest));
            var result = await loginResponse.Content.ReadAsAsync<AuthenticatedResponse>();
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + result.Token);

            var response = await _client.PostAsync("users/refresh-token", null);
            var refreshResult = await response.Content.ReadAsAsync<AuthenticatedResponse>();
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization","Bearer " + refreshResult.Token);
            var getResponse = await _client.GetAsync("users");

            response.EnsureSuccessStatusCode();
            getResponse.EnsureSuccessStatusCode();
        }
    }
}
