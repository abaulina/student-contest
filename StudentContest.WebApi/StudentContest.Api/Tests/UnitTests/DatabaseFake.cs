using System;
using Microsoft.EntityFrameworkCore;
using StudentContest.Api.Models;
using StudentContest.Api.Tests.Helpers;

namespace StudentContest.Api.Tests.UnitTests
{
    internal class DatabaseFake: IDisposable
    {
        private readonly AuthenticationContext _context;

        public DatabaseFake()
        {
            var options = new DbContextOptionsBuilder<AuthenticationContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _context = new AuthenticationContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            Utilities.InitializeDbForTests(_context);
        }

        public AuthenticationContext GetContext()
        {
            return _context;
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
