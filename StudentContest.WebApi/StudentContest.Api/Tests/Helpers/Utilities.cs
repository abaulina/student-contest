using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using StudentContest.Api.Helpers;
using StudentContest.Api.Models;

namespace StudentContest.Api.Tests.Helpers
{
    internal class Utilities
    {
        public static void InitializeDbForTests(AuthenticationContext db)
        {
            db.Users.AddRange(Seed());
            db.RefreshTokens.AddRange(SeedRefreshTokens());
            db.SaveChanges();
        }

        public static StringContent GetStringContent(object obj)
            => new(JsonConvert.SerializeObject(obj), Encoding.Default, "application/json");
        
        private static IEnumerable<User> Seed()
        {
            var passwordHasher = new BCryptPasswordHasher();
            var users = new User[] {
                new()
                {
                    Email = "test@example.com", FirstName = "Test", LastName = "User", PasswordHash = passwordHasher.HashPassword("12345678")
                },
                new()
                {
                    Email = "user@example.com", FirstName = "Test", LastName = "User", PasswordHash = passwordHasher.HashPassword("12345678")
                },
                new()
                {
                    Email = "first@example.com", FirstName = "Test", LastName = "User", PasswordHash = passwordHasher.HashPassword("12345678")
                },
                new()
                {
                    Email = "second@example.com", FirstName = "Test", LastName = "User", PasswordHash = passwordHasher.HashPassword("12345678")
                }
            };
            return users;
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
