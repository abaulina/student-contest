using StudentContest.Api.Models;

namespace StudentContest.Api.Services
{
    public interface IUserRolesService
    {
        Task<IList<string>> GetUserRoles(int userId);
        Task<IList<UserRoles>> GetUsersWithRoles();
        Task AssignRole(int userId, string role);
        Task UnassignRole(int userId, string role);
    }

    public class UserRolesService: IUserRolesService
    {
        private readonly IUserManagerWrapper _userManagerWrapper;

        public UserRolesService(IUserManagerWrapper userManagerWrapper)
        {
            _userManagerWrapper = userManagerWrapper;
        }

        public async Task<IList<string>> GetUserRoles(int userId)
        {
            return await _userManagerWrapper.GetUserRolesAsync(userId);
        }

        public async Task<IList<UserRoles>> GetUsersWithRoles()
        {
            return await _userManagerWrapper.GetRolesAsync();
        }

        public async Task AssignRole(int userId, string role)
        {
            await _userManagerWrapper.AddToRoleAsync(userId, role);
        }

        public async Task UnassignRole(int userId, string role)
        {
            await _userManagerWrapper.RemoveFromRoleAsync(userId, role);
        }
    }
}
