using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using StudentContest.Api.Models;
using StudentContest.Api.Services;

namespace StudentContest.Api.Controllers
{
    [EnableCors("ApiCorsPolicy")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserRolesController: ControllerBase
    {
        private readonly IUserRolesService _userRolesService;

        public UserRolesController(IUserRolesService userRolesService)
        {
            _userRolesService = userRolesService;
        }

        [HttpPost("user/{userId:int}/assign")]
        public async Task<IActionResult> AssignRole(int userId, [FromBody] string roleName)
        {
            await _userRolesService.AssignRole(userId, roleName);
            return Ok();
        }

        [HttpPost("user/{userId:int}/unassign")]
        public async Task<IActionResult> UnassignRole(int userId, [FromBody] string roleName)
        {
            await _userRolesService.UnassignRole(userId, roleName);
            return Ok();
        }

        [HttpGet("user/{userId:int}/roles")]
        public async Task<ActionResult<User>> GetUserRoles(int userId)
        {
            var userRoles = await _userRolesService.GetUserRoles(userId);
            return Ok(userRoles);
        }

        [HttpGet]
        public async Task<ActionResult<User>> GetUsersWithRoles()
        {
            var usersRoles = await _userRolesService.GetUsersWithRoles();
            return Ok(usersRoles);
        }
    }
}
