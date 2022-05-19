using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using StudentContest.Api.Models;

namespace StudentContest.Api.Tests.Helpers
{
    internal class Utilities
    {
        public static void InitializeDbForTests(ApplicationContext db)
        {
            db.Users.AddRange(Seed());
            db.RefreshTokens.AddRange(SeedRefreshTokens());
            db.SaveChanges();
        }

        public static async Task InitializeDbForIntegrationTests(ApplicationContext db,
            UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole<int>("Admin"));
            await roleManager.CreateAsync(new IdentityRole<int>("User"));

            var users = Seed();
            await AddToUserRole(users, userManager);
            await userManager.AddToRoleAsync(users.Last(), "Admin");

            db.Users.AddRange(users);
            db.RefreshTokens.AddRange(SeedRefreshTokens());

            await db.SaveChangesAsync();
        }

        public static StringContent GetStringContent(object obj)
            => new(JsonConvert.SerializeObject(obj), Encoding.Default, "application/json");
        
        private static IEnumerable<User> Seed()
        {
            var users = new User[] {
                new()
                {
                    Id = 1, Email = "test@example.com", FirstName = "Test", LastName = "User", PasswordHash = "12345678"
                },
                new()
                {
                    Id = 2,Email = "user@example.com", FirstName = "Test", LastName = "User", PasswordHash = "12345678"
                },
                new()
                {
                    Id = 3, Email = "first@example.com", FirstName = "Test", LastName = "User", PasswordHash = "12345678"
                },
                new()
                {
                    Id = 4,Email = "second@example.com", FirstName = "Test", LastName = "User", PasswordHash = "12345678"
                }
            };
            return users;
        }

        private static async Task AddToUserRole(IEnumerable<User> users, UserManager<User> userManager)
        {
            foreach(var user in users)
                await userManager.AddToRoleAsync(user, "User");
        }

        private static IEnumerable<RefreshToken> SeedRefreshTokens()
        {
            var refreshTokens = new List<RefreshToken>
            {
                new() {UserId = 3, Token = "3token"},
                new()
                {
                    UserId = 4, Token = "notRevoked"
                }
            };
            return refreshTokens;
        }
    }
}
