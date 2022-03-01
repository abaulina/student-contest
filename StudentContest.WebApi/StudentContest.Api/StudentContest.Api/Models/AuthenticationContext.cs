using Microsoft.EntityFrameworkCore;

namespace StudentContest.Api.Models
{
    public class AuthenticationContext: DbContext
    {
        public AuthenticationContext(DbContextOptions<AuthenticationContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    }
}
