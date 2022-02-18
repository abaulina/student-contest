using System;
using Microsoft.EntityFrameworkCore;
using StudentContest.Api.Models;
using StudentContest.Api.Tests.Helpers;

namespace StudentContest.Api.Tests.UnitTests
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
            Utilities.InitializeDbForTests(_context);
        }

        public UserContext GetContext()
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
