using Microsoft.EntityFrameworkCore;
using StudentContest.Api.Models;

namespace StudentContest.Api.Services.UserRepository
{
    public class DatabaseUserRepository : IUserRepository
    {
        private readonly AuthenticationContext _context;

        public DatabaseUserRepository(AuthenticationContext context)
        {
            _context = context;
        }

        public async Task Add(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> Find(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }
    }
}
