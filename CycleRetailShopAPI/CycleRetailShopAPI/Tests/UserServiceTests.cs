using CycleRetailShopAPI.Data;
using CycleRetailShopAPI.Models;
using CycleRetailShopAPI.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CycleRetailShopAPI.Tests
{
    [TestFixture]
    public class UserServiceTests
    {
        private ApplicationDbContext _context;
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            // Seed test users
            _context.Users.AddRange(
                new User { UserID = 99, Email = "admin1@example.com", Role = UserRole.Admin },
                new User { UserID = 100, Email = "employee1@example.com", Role = UserRole.Employee },
                new User { UserID = 101, Email = "admin2@example.com", Role = UserRole.Admin }
            );

            _context.SaveChanges();

            _userService = new UserService(_context);
        }

        [Test]
        public async Task GetAdminEmails_ShouldReturnOnlyAdminEmails()
        {
            // Act
            var result = await _userService.GetAdminEmails();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result, Does.Contain("admin1@example.com"));
            Assert.That(result, Does.Contain("admin2@example.com"));
            Assert.That(result, Does.Not.Contain("employee1@example.com"));
        }
    }
}
