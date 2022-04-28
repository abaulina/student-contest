using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace StudentContest.Api.Models
{
    public class AuthenticationContext: IdentityDbContext<User, IdentityRole<int>, int>
    {
        public AuthenticationContext(DbContextOptions<AuthenticationContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder
                .Entity<User>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
            modelBuilder
                .Entity<IdentityRole>()
                .HasData(new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER"
                }, new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                });
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    }
}
