#nullable disable
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using StudentContest.Api.Models;
using StudentContest.Api.Services;

namespace StudentContest.Api.Controllers
{
    [EnableCors("ApiCorsPolicy")]
    [Route("users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var userInfo = await _userService.GetUserInfo(id);
            return Ok(userInfo);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<User>> GetUser()
        {
            var userId = HttpContext.User.FindFirstValue("id");
            if (!int.TryParse(userId, out var id))
            {
                return Unauthorized();
            }

            var userInfo = await _userService.GetUserInfo(id);
            return Ok(userInfo);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _userService.GetUsers();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest registerRequest)
        {
            var user = await _userService.Register(registerRequest);
            return CreatedAtAction(nameof(GetUser), user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequest registerRequest)
        {
            var admin = await _userService.Register(registerRequest, "Admin");
            return CreatedAtAction(nameof(GetUser), admin);
        }
    }
}
