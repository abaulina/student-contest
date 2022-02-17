using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using StudentContest.Api.Models;

namespace Tests
{
    internal class UsersDatabaseFake: IDisposable
    {
        private readonly UserContext _context;

        public UsersDatabaseFake()
        {
            var options = new DbContextOptionsBuilder<UserContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _context = new UserContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            Seed();
        }

        public UserContext GetContext()
        {
            return _context;
        }

        private void Seed()
        {
            var users = new User[] {
                new()
                {
                    Email = "test@example.com", FirstName = "Test", LastName = "User", PasswordHash = "12345678"
                },
                new()
                {
                    Email = "user@example.com", FirstName = "Test", LastName = "User", PasswordHash = "12345678"
                },
                new()
                {
                    Email = "first@example.com", FirstName = "Test", LastName = "User", PasswordHash = "12345678", RefreshTokens = new List<RefreshToken> {new() {Token = "alreadyRevoked", Revoked = DateTime.Now.AddMinutes(-5)}}
                },
                new()
                {
                    Email = "second@example.com", FirstName = "Test", LastName = "User", PasswordHash = "12345678", RefreshTokens = new List<RefreshToken> {new() {Token = "notRevoked", Expires = DateTime.Now.AddMinutes(5)}}
                },
            };
            _context.Users.AddRange(users);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
