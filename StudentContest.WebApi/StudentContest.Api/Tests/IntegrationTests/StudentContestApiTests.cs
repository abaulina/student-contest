using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
        public async Task Get_UnauthorizedAccess_NotAllowed()
        {
            var response = await _client.GetAsync("users/0");

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Post_RegisterUser_ReturnsOk()
        {
            var registerRequest = new RegisterRequest
                {Email = "notUsed@example.com", FirstName = "Test", LastName = "User", Password = "12345678"};
            
            var response = await _client.PostAsync("users/register", Utilities.GetStringContent(registerRequest));

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
