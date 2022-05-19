using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentContest.Api.Models;

namespace StudentContest.Api.Services
{
    public interface IUserManagerWrapper
    {
        Task<User?> FindByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<IdentityResult> CreateAsync(User user, string password);
        Task<User?> GetUserInfoAsync(int id);
        Task<IEnumerable<User>> GetUsersAsync();
        Task<IdentityResult> AddToRoleAsync(int userId, string roleName);
        Task<IList<string>> GetUserRolesAsync(int userId);
        Task<IList<UserRoles>> GetRolesAsync();
        Task <IdentityResult> RemoveFromRoleAsync(int userId, string roleName);
    }

    public class UserManagerWrapper : IUserManagerWrapper
    {
        private readonly UserManager<User> _userManager;

        public UserManagerWrapper(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }
        
        public async Task<IdentityResult> CreateAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<User?> GetUserInfoAsync(int id)
        {
            var user = await GetUserAsync(id);
            return new User
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id
            };
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _userManager.Users.Select(u => new User
                {Email = u.Email, FirstName = u.FirstName, LastName = u.LastName, Id = u.Id}).ToListAsync();
        }

        public async Task<IdentityResult> AddToRoleAsync(int userId, string roleName)
        {
            var user = await GetUserAsync(userId);
            return await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IList<string>> GetUserRolesAsync(int userId)
        {
            var user = await GetUserAsync(userId);
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<IList<UserRoles>> GetRolesAsync()
        {
            return await _userManager.Users.Select(c => new UserRoles()
            {
                User = c,
                Roles = new List<string>(_userManager.GetRolesAsync(c).Result.ToList())
            }).ToListAsync();
        }

        public async Task<IdentityResult> RemoveFromRoleAsync(int userId, string roleName)
        {
            var user = await GetUserAsync(userId);
            return await _userManager.RemoveFromRoleAsync(user, roleName);
        }

        private async Task<User> GetUserAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new KeyNotFoundException();
            return user;
        }
    }
}
