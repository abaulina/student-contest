using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentContest.Api.Services;

namespace StudentContest.Api.Controllers
{
    [EnableCors("ApiCorsPolicy")]
    [Route("roles")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost]
        public async Task<IActionResult> AddRole([FromBody] string roleName)
        {
            var role = await _roleService.AddRoleAsync(roleName);
            return CreatedAtAction(nameof(GetRole), new {id = role.Id});
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<IdentityRole>>> GetAllRoles()
        {
            var roles = await _roleService.GetRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<IdentityRole>> GetRole(int id)
        {
            var role = await _roleService.GetRoleAsync(id);
            return Ok(role);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            await _roleService.DeleteRoleAsync(id);
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> EditRole(int id, [FromBody] string roleName)
        {
            await _roleService.EditRole(id, roleName);
            return Ok();
        }
    }
}
