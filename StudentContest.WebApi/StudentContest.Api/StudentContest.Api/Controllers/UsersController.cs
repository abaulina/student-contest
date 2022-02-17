#nullable disable
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
    [Authorization.Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var userInfo = await _userService.GetCurrentUserInfo(id);
            return Ok(userInfo);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest registerRequest)
        {
            await _userService.Register(registerRequest);
            return Ok();
        }

        [AllowAnonymous]
        [HttpDelete("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            await _userService.Logout(refreshToken, IpAddress());
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var response = await _userService.Login(loginRequest, IpAddress());
            SetTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = await _userService.RefreshToken(refreshToken, IpAddress());
            SetTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
