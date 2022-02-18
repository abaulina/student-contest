using System;
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
        public static void InitializeDbForTests(UserContext db)
        {
            db.Users.AddRange(Seed());
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
                    Email = "first@example.com", FirstName = "Test", LastName = "User", PasswordHash = passwordHasher.HashPassword("12345678"), RefreshTokens = new List<RefreshToken> {new() {Token = "alreadyRevoked", Revoked = DateTime.Now.AddMinutes(-5)}}
                },
                new()
                {
                    Email = "second@example.com", FirstName = "Test", LastName = "User", PasswordHash = passwordHasher.HashPassword("12345678"), RefreshTokens = new List<RefreshToken> {new() {Token = "notRevoked", Expires = DateTime.Now.AddMinutes(5)}}
                }
            };
            return users;
        }
    }
}
