using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace StudentContest.Api.Models
{
    public class ApplicationContext: IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
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
                .Entity<IdentityRole<int>>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<UserNote>()
                .HasKey(un => new { un.UserId, un.NoteId });

            modelBuilder.Entity<UserNote>()
                .HasOne(un => un.User)
                .WithMany(u => u.UserNotes)
                .HasForeignKey(un => un.UserId);

            modelBuilder.Entity<UserNote>()
                .HasOne(un => un.Note)
                .WithMany(n => n.UserNotes)
                .HasForeignKey(un => un.NoteId);
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Note?> Notes { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    }
}
