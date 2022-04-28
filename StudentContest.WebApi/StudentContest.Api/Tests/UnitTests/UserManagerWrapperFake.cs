using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentContest.Api.Models;
using StudentContest.Api.Services;

namespace StudentContest.Api.Tests.UnitTests
{
    internal class TestUserManagerWrapper : IUserManagerWrapper
    {
        private readonly AuthenticationContext _context;

        public TestUserManagerWrapper(AuthenticationContext context)
        {
            _context = context;
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => string.Compare(x.Email, email, StringComparison.Ordinal) == 0);
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            if (user == null) throw new InvalidCredentialException();
            var dbUser = await _context.Users.FindAsync(user.Id);
            return string.Compare(dbUser?.PasswordHash, password, StringComparison.Ordinal) == 0;
        }

        public async Task<IdentityResult> CreateAsync(User user, string password)
        {
            var userToAdd = new User
                {PasswordHash = password, FirstName = user.FirstName, LastName = user.LastName, Email = user.Email};
            _context.Users.Add(userToAdd);
            await _context.SaveChangesAsync();
            return IdentityResult.Success;
        }

        public async Task<User?> FindByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<IEnumerable<User>> FindAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public Task<IdentityResult> AddToRoleAsync(User user, string roleName)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IList<string>> GetRolesAsync(User user)
        {
            return Task.FromResult<IList<string>>(new List<string> {"User"});
        }
    }
}
