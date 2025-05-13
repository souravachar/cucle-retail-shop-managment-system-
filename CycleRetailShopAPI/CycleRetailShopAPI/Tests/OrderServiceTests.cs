using NUnit.Framework;
using Moq;
using CycleRetailShopAPI.Services;
using CycleRetailShopAPI.Data;
using CycleRetailShopAPI.Interfaces;
using CycleRetailShopAPI.Models;
using CycleRetailShopAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CycleRetailShopAPI.Tests
{
    public class OrderServiceTests
    {
        private ApplicationDbContext _context;
        private Mock<ICustomerService> _customerServiceMock;
        private OrderService _orderService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _customerServiceMock = new Mock<ICustomerService>();

            _orderService = new OrderService(_context, _customerServiceMock.Object);
        }

        [Test]
        public void PlaceOrder_ValidOrder_ShouldSaveOrder()
        {
            // Arrange
            var cycle = new Cycle { CycleID = 1, ModelName = "Speedster", Price = 10000, StockQuantity = 5 };
            _context.Cycles.Add(cycle);
            _context.SaveChanges();

            var customer = new Customer { CustomerID = 1, FullName = "John", PhoneNumber = "1234567890", Email = "john@example.com" };
            _customerServiceMock.Setup(x => x.GetOrCreateCustomer(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(customer);

            var orderDto = new OrderDTO
            {
                CustomerName = "John",
                CustomerPhone = "1234567890",
                CustomerEmail = "john@example.com",
                NewCustomerAddress = "123 Street",
                EmployeeID = 1,
                OrderDetails = new List<CycleRetailShopAPI.Models.DTO.OrderDetailDTO>
            {
                new CycleRetailShopAPI.Models.DTO.OrderDetailDTO { CycleID = 1, Quantity = 2 }
            }
            };


            // Act
            var result = _orderService.PlaceOrder(orderDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CustomerID, Is.EqualTo(1));
            Assert.That(result.TotalAmount, Is.EqualTo(20000));
            Assert.That(result.Status, Is.EqualTo(OrderStatus.Pending));

        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
