using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Castle.Core.Internal;
using StudentContest.Api.Models;
using StudentContest.Api.Tests.Helpers;
using Xunit;

namespace StudentContest.Api.Tests.IntegrationTests
{
    public class StudentContestApiTests : IClassFixture<ApiWebAppFactory>
    {
        private readonly HttpClient _client;

        public StudentContestApiTests(ApiWebAppFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetUser_UnauthorizedAccess_NotAllowed()
        {
            var response = await _client.GetAsync("users");

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAllUsers_UnauthorizedAccess_NotAllowed()
        {
            var response = await _client.GetAsync("users/all");

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task RegisterUser_Success_ReturnsOk()
        {
            var registerRequest = new RegisterRequest
                {Email = GenerateEmail(), FirstName = "Test", LastName = "User", Password = "12345678"};
            var loginRequest = new LoginRequest{ Email = registerRequest.Email, Password = registerRequest.Password };

            var response = await _client.PostAsync("users/register", Utilities.GetStringContent(registerRequest));
            var loginResponse = await _client.PostAsync("users/login", Utilities.GetStringContent(loginRequest));

            response.EnsureSuccessStatusCode();
            loginResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task RegisterAdmin_IncorrectBodyType_ReturnsBadRequest()
        {
            var registerRequest = new {Email = "new@example.com", Password = "12345678", Username = "user"};

            var response = await _client.PostAsync("users/register-admin", Utilities.GetStringContent(registerRequest));

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RegisterAdmin_Success_ReturnsOk()
        {
            var registerRequest = new RegisterRequest
            { Email = GenerateEmail(), FirstName = "Test", LastName = "User", Password = "12345678" };
            var loginRequest = new LoginRequest { Email = registerRequest.Email, Password = registerRequest.Password };

            var response = await _client.PostAsync("users/register-admin", Utilities.GetStringContent(registerRequest));
            var loginResponse = await _client.PostAsync("users/login", Utilities.GetStringContent(loginRequest));

            response.EnsureSuccessStatusCode();
            loginResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task RegisterUser_IncorrectBodyType_ReturnsBadRequest()
        {
            var registerRequest = new { Email = "new@example.com", Password = "12345678", Username = "user" };

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

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            var loginRequest = new LoginRequest
                { Email = "t@example.com", Password = "12345678" };
            _client.DefaultRequestHeaders.Add("X-Forwarded-For", "ipAddress");

            var response = await _client.PostAsync("users/login", Utilities.GetStringContent(loginRequest));

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_Success_ReturnsOkAndValidTokens()
        {
            var registerRequest = new RegisterRequest
                { Email = GenerateEmail(), FirstName = "Test", LastName = "User", Password = "12345678" };
            var loginRequest = new LoginRequest { Email = registerRequest.Email, Password = registerRequest.Password };

            var registerResponse = await _client.PostAsync("users/register", Utilities.GetStringContent(registerRequest));

            registerResponse.EnsureSuccessStatusCode();

            var loginResponse = await _client.PostAsync("users/login", Utilities.GetStringContent(loginRequest));
            var result = await loginResponse.Content.ReadAsAsync<AuthenticatedResponse>();
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer "+result.Token);
            
            loginResponse.EnsureSuccessStatusCode();

            var getResponse = await _client.GetAsync("users");
            var getResponseResult = await getResponse.Content.ReadAsAsync<User>();

            getResponse.EnsureSuccessStatusCode();
            Assert.Equal(loginRequest.Email, getResponseResult.Email);
        }
        
        [Fact]
        public async Task LoginAsUser_Success_GetAllUsers_Forbidden()
        {
            var registerRequest = new RegisterRequest
            { Email = GenerateEmail(), FirstName = "Test", LastName = "User", Password = "12345678" };
            var loginRequest = new LoginRequest { Email = registerRequest.Email, Password = registerRequest.Password };

            var registerResponse = await _client.PostAsync("users/register", Utilities.GetStringContent(registerRequest));

            registerResponse.EnsureSuccessStatusCode();

            var loginResponse = await _client.PostAsync("users/login", Utilities.GetStringContent(loginRequest));
            var result = await loginResponse.Content.ReadAsAsync<AuthenticatedResponse>();
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + result.Token);

            loginResponse.EnsureSuccessStatusCode();

            var getResponse = await _client.GetAsync("users/all");

            Assert.False(getResponse.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, getResponse.StatusCode);
        }

        [Fact]
        public async Task LoginAdmin_Success_GetUser_Forbidden()
        {
            var registerRequest = new RegisterRequest
                { Email = GenerateEmail(), FirstName = "Test", LastName = "User", Password = "12345678" };
            var loginRequest = new LoginRequest { Email = registerRequest.Email, Password = registerRequest.Password };

            var registerResponse = await _client.PostAsync("users/register-admin", Utilities.GetStringContent(registerRequest));

            registerResponse.EnsureSuccessStatusCode();

            var loginResponse = await _client.PostAsync("users/login", Utilities.GetStringContent(loginRequest));
            var result = await loginResponse.Content.ReadAsAsync<AuthenticatedResponse>();
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + result.Token);

            loginResponse.EnsureSuccessStatusCode();

            var getResponse = await _client.GetAsync("users");

            Assert.False(getResponse.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, getResponse.StatusCode);
        }

        [Fact]
        public async Task LoginAdmin_Success_ReturnsOkAndValidTokens()
        {
            var registerRequest = new RegisterRequest
                { Email = GenerateEmail(), FirstName = "Test", LastName = "User", Password = "12345678" };
            var loginRequest = new LoginRequest { Email = registerRequest.Email, Password = registerRequest.Password };

            var registerResponse = await _client.PostAsync("users/register-admin", Utilities.GetStringContent(registerRequest));

            registerResponse.EnsureSuccessStatusCode();

            var loginResponse = await _client.PostAsync("users/login", Utilities.GetStringContent(loginRequest));
            var result = await loginResponse.Content.ReadAsAsync<AuthenticatedResponse>();
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + result.Token);

            loginResponse.EnsureSuccessStatusCode();

            var getResponse = await _client.GetAsync("users/all");
            var getResponseResult = await getResponse.Content.ReadAsAsync<IEnumerable<User>>();

            getResponse.EnsureSuccessStatusCode();
            Assert.False(getResponseResult.IsNullOrEmpty());
        }

        [Fact]
        public async Task Logout_NoTokenInCookies_ReturnsBadRequest()
        {
            var response = await _client.DeleteAsync("users/logout");

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Logout_Success_ImpossibleToReuseToken()
        {
            var registerRequest = new RegisterRequest
                { Email = GenerateEmail(), FirstName = "Test", LastName = "User", Password = "12345678" };
            var loginRequest = new LoginRequest { Email = registerRequest.Email, Password = registerRequest.Password };

            var registerResponse = await _client.PostAsync("users/register", Utilities.GetStringContent(registerRequest));

            registerResponse.EnsureSuccessStatusCode();

            var loginResponse = await _client.PostAsync("users/login", Utilities.GetStringContent(loginRequest));

            loginResponse.EnsureSuccessStatusCode();

            var setCookieHeader = loginResponse.Headers.NonValidated.Last();
            _client.DefaultRequestHeaders.Add("Cookie", setCookieHeader.Value);

            var logoutResponse = await _client.DeleteAsync("users/logout");
            var refreshResponse = await _client.PostAsync("users/refresh-token", null);

            logoutResponse.EnsureSuccessStatusCode();
            Assert.False(refreshResponse.IsSuccessStatusCode);
        }

        [Fact]
        public async Task RefreshToken_InvalidToken_ReturnsBadRequest()
        {
            var response = await _client.PostAsync("users/refresh-token", null);

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        
        [Fact]
        public async Task RefreshToken_Success_NewTokenIsValid()
        {
            var registerRequest = new RegisterRequest
                { Email = GenerateEmail(), FirstName = "Test", LastName = "User", Password = "12345678" };
            var loginRequest = new LoginRequest { Email = registerRequest.Email, Password = registerRequest.Password };

            var registerResponse = await _client.PostAsync("users/register", Utilities.GetStringContent(registerRequest));

            registerResponse.EnsureSuccessStatusCode();

            var loginResponse = await _client.PostAsync("users/login", Utilities.GetStringContent(loginRequest));

            loginResponse.EnsureSuccessStatusCode();

            var setCookieHeader = loginResponse.Headers.NonValidated.Last();
            _client.DefaultRequestHeaders.Add("Cookie", setCookieHeader.Value);

            var refreshResponse = await _client.PostAsync("users/refresh-token", null);

            refreshResponse.EnsureSuccessStatusCode();

            _client.DefaultRequestHeaders.Clear();
            setCookieHeader = refreshResponse.Headers.NonValidated.Last();
            _client.DefaultRequestHeaders.Add("Cookie", setCookieHeader.Value);

            var refreshResponseWithNewToken = await _client.PostAsync("users/refresh-token", null);
            
            refreshResponseWithNewToken.EnsureSuccessStatusCode();
        }

        private static string GenerateEmail()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz1234567890";
            var str = "";
            var random = new Random();
            for (var i = 0; i < 15; i++)
            {
                str += chars[random.Next() % chars.Length];
            }
            return str + "@domain.com";
        }
    }
}
