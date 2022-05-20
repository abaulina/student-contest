using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentContest.Api.Models;
using StudentContest.Api.Services;
using StudentContest.Api.Validation;
using Xunit;

namespace StudentContest.Api.Tests.UnitTests
{
    public class UserServiceTests
    {
        private readonly ApplicationContext _context;
        private readonly IUserManagerWrapper _userManager;

        public UserServiceTests()
        {
            _context = new DatabaseFake().GetContext();
            _userManager = new TestUserManagerWrapper(_context);
        }

        [Fact]
        public async Task GetUserInfo_NonExistingUser_ThrowsException()
        {
            var userService = new UserService(_userManager, new RegisterRequestValidator());

            await Assert.ThrowsAsync<KeyNotFoundException>(() => userService.GetUserInfo(-1));
        }

        [Fact]
        public async Task GetUserInfo_Success_CorrectData()
        {
            var userService = new UserService(_userManager, new RegisterRequestValidator());

            var result = await userService.GetUserInfo(1);

            Assert.Equal(1, result.Id);
            Assert.Equal("test@example.com", result.Email);
        }

        [Fact]
        public async Task Register_Success_ChangesDatabase()
        {
            var userService = new UserService(_userManager, new RegisterRequestValidator());
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
        public async Task GetUsers_Success_ReturnsAllUsers()
        {
            var userService = new UserService(_userManager, new RegisterRequestValidator());

            var result = await userService.GetUsers();

            Assert.Equal(_context.Users.Count(), result.Count());
        }
    }
}
