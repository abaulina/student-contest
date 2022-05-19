using Microsoft.AspNetCore.Identity;

namespace StudentContest.Api.Services
{
    public interface IRoleService
    {
        Task<IdentityRole<int>> AddRoleAsync(string roleName);
        Task DeleteRoleAsync(int roleId);
        Task EditRole(int roleId, string roleName);
        Task<List<IdentityRole<int>>> GetRolesAsync();
        Task<IdentityRole<int>> GetRoleAsync(int roleId);
    }


    public class RoleService:IRoleService
    {
        private readonly IRoleManagerWrapper _roleManagerWrapper;

        public RoleService(IRoleManagerWrapper roleManagerWrapper)
        {
            _roleManagerWrapper = roleManagerWrapper;
        }

        public async Task DeleteRoleAsync(int roleId)
        {
            await _roleManagerWrapper.DeleteRoleAsync(roleId);
        }

        public Task EditRole(int roleId, string roleName)
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityRole<int>> AddRoleAsync(string roleName)
        {
            ValidateRoleName(roleName);
            return await _roleManagerWrapper.CreateRoleAsync(roleName);
        }

        public async Task<List<IdentityRole<int>>> GetRolesAsync()
        {
            return await _roleManagerWrapper.GetRolesAsync();
        }

        public async Task<IdentityRole<int>> GetRoleAsync(int roleId)
        {
            return await _roleManagerWrapper.GetRoleByIdAsync(roleId);
        }

        private void ValidateRoleName(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException(roleName);
        }
    }
}
