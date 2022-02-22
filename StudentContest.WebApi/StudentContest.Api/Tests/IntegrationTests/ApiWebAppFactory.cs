﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StudentContest.Api.Models;
using StudentContest.Api.Tests.Helpers;

namespace StudentContest.Api.Tests.IntegrationTests
{
    public class ApiWebAppFactory<TEntryPoint> : WebApplicationFactory<Program> where TEntryPoint : Program
    {
        private readonly LoggerFake _loggerFake = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var defaultILogger = services.SingleOrDefault(d => d.ServiceType == typeof(ILogger));
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<AuthenticationContext>));

                services.Remove(descriptor);
                services.Remove(defaultILogger);

                services.AddSingleton<ILogger>(_loggerFake);

                services.AddDbContext<AuthenticationContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });
                
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                using var authenticationContext = scope.ServiceProvider.GetRequiredService<AuthenticationContext>();
                try
                {
                    authenticationContext.Database.EnsureCreated();
                    Utilities.InitializeDbForTests(authenticationContext);
                }
                catch (Exception ex)
                {
                    _loggerFake.LogError(ex, "An error occurred seeding the " +
                                             "database with test messages. Error: {Message}", ex.Message);
                }
            });
        }
    }
}
