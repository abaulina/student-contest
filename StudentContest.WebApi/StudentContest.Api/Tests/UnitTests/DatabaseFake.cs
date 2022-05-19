using System;
using Microsoft.EntityFrameworkCore;
using StudentContest.Api.Models;
using StudentContest.Api.Tests.Helpers;

namespace StudentContest.Api.Tests.UnitTests
{
    internal class DatabaseFake: IDisposable
    {
        private readonly ApplicationContext _context;

        public DatabaseFake()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _context = new ApplicationContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            Utilities.InitializeDbForTests(_context);
        }

        public ApplicationContext GetContext()
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
