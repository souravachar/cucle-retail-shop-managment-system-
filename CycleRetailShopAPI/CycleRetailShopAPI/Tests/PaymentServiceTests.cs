using CycleRetailShopAPI.Data;
using CycleRetailShopAPI.DTOs;
using CycleRetailShopAPI.Interfaces;
using CycleRetailShopAPI.Models;
using CycleRetailShopAPI.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CycleRetailShopAPI.Tests
{
    [TestFixture]
    public class PaymentServiceTests
    {
        private ApplicationDbContext _context;
        private PaymentService _paymentService;
        private Mock<IStripeService> _mockStripeService;
        private Mock<IUPIService> _mockUPIService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockStripeService = new Mock<IStripeService>();
            _mockUPIService = new Mock<IUPIService>();
            _paymentService = new PaymentService(_context, _mockStripeService.Object, _mockUPIService.Object);
        }

        [Test]
        public void ProcessPayment_ShouldAddPayment_AndUpdateOrderStatus()
        {
            // Arrange
            var order = new Order { OrderID = 1, TotalAmount = 500, Status = OrderStatus.Pending };
            _context.Orders.Add(order);
            _context.SaveChanges();

            var paymentDto = new PaymentDTO
            {
                OrderID = 1,
                CustomerID = 2,
                Method = PaymentMethod.Cash,
                Status = PaymentStatus.Success
            };

            // Act
            var result = _paymentService.ProcessPayment(paymentDto);

            // Assert
            var payment = _context.Payments.FirstOrDefault();
            var updatedOrder = _context.Orders.FirstOrDefault();

            Assert.That(payment, Is.Not.Null);
            Assert.That(payment.OrderID, Is.EqualTo(1));
            Assert.That(payment.TransactionID, Is.Not.Null);
            Assert.That(updatedOrder.Status, Is.EqualTo(OrderStatus.Delivered));

        }

        [Test]
        public void GetPaymentByOrderId_ShouldReturnCorrectPayment()
        {
            // Arrange
            var payment = new Payment { PaymentID = 1, OrderID = 1, Method = PaymentMethod.Cash, Status = PaymentStatus.Success };
            _context.Payments.Add(payment);
            _context.SaveChanges();

            // Act
            var result = _paymentService.GetPaymentByOrderId(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.OrderID, Is.EqualTo(1));
        }

        [Test]
        public void GetAllPayments_ShouldReturnAllPaymentsWithOrderDetails()
        {
            // Arrange
            var order = new Order { OrderID = 1, TotalAmount = 2500 };
            var payment = new Payment
            {
                PaymentID = 1,
                OrderID = 1,
                Method = PaymentMethod.Card,
                Status = PaymentStatus.Success,
                TransactionID = "TX123"
            };
            _context.Orders.Add(order);
            _context.Payments.Add(payment);
            _context.SaveChanges();

            // Act
            var result = _paymentService.GetAllPayments();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Amount, Is.EqualTo(2500));
            Assert.That(result[0].TransactionID, Is.EqualTo("TX123"));
        }

        [Test]
        public async Task CreatePaymentIntent_ShouldCallStripe_WhenMethodIsCard()
        {
            // Arrange
            var dto = new PaymentDTO
            {
                OrderID = 1,
                CustomerID = 1,
                Method = PaymentMethod.Card,
                Amount = 1200
            };

            _mockStripeService.Setup(x => x.CreatePaymentIntent(dto))
                              .ReturnsAsync("stripe-client-secret");

            // Act
            var result = await _paymentService.CreatePaymentIntent(dto);

            // Assert
            Assert.That(result, Is.EqualTo("stripe-client-secret"));
        }

        [Test]
        public async Task CreatePaymentIntent_ShouldCallUPI_WhenMethodIsUPI()
        {
            // Arrange
            var dto = new PaymentDTO
            {
                OrderID = 1,
                CustomerID = 1,
                Method = PaymentMethod.UPI,
                Amount = 999
            };

            _mockUPIService.Setup(x => x.CreateUPIPayment(dto))
                           .ReturnsAsync("upi-txn-id");

            // Act
            var result = await _paymentService.CreatePaymentIntent(dto);

            // Assert
            Assert.That(result, Is.EqualTo("upi-txn-id"));
        }

        [Test]
        public void CreatePaymentIntent_ShouldThrowException_ForUnsupportedMethod()
        {
            // Arrange
            var dto = new PaymentDTO
            {
                Method = (PaymentMethod)999,
                OrderID = 1,
                CustomerID = 1,
                Amount = 100
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotSupportedException>(async () =>
                await _paymentService.CreatePaymentIntent(dto));

            Assert.That(ex.Message, Is.EqualTo("Payment method not supported"));
        }
    }
}
