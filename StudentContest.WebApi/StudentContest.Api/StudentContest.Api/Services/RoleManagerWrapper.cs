using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace StudentContest.Api.Services
{
    public interface IRoleManagerWrapper
    {
        Task<IdentityRole<int>> CreateRoleAsync(string roleName);
        Task<IdentityResult> DeleteRoleAsync(int roleId);
        Task<List<IdentityRole<int>>> GetRolesAsync();
        Task<IdentityRole<int>> GetRoleByIdAsync(int roleId);
    }


    public class RoleManagerWrapper: IRoleManagerWrapper
    {
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public RoleManagerWrapper(RoleManager<IdentityRole<int>> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IdentityRole<int>> CreateRoleAsync(string roleName)
        {
            await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
            return await _roleManager.FindByNameAsync(roleName);
        }

        public async Task<IdentityResult> DeleteRoleAsync(int roleId)
        {
            var role = await GetRoleByIdAsync(roleId);
            return await _roleManager.DeleteAsync(role);
        }

        public async Task<List<IdentityRole<int>>> GetRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task<IdentityRole<int>> GetRoleByIdAsync(int roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
                throw new KeyNotFoundException();
            return role;
        }
    }
}
