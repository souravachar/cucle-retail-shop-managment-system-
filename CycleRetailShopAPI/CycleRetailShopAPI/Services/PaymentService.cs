using CycleRetailShopAPI.Data;
using CycleRetailShopAPI.Interfaces;
using CycleRetailShopAPI.Models;
using CycleRetailShopAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Add this alias to resolve the ambiguity
using PaymentMethod = CycleRetailShopAPI.Models.PaymentMethod;

namespace CycleRetailShopAPI.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IStripeService _stripeService;
        private readonly IUPIService _upiService;

        public PaymentService(
            ApplicationDbContext context,
            IStripeService stripeService,
            IUPIService upiService)
        {
            _context = context;
            _stripeService = stripeService;
            _upiService = upiService;
        }

        public Payment ProcessPayment(PaymentDTO paymentDto)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderID == paymentDto.OrderID);
            if (order == null)
                throw new KeyNotFoundException("Order not found");

            var payment = new Payment
            {
                OrderID = paymentDto.OrderID,
                Method = paymentDto.Method,
                TransactionID = paymentDto.TransactionID ?? GenerateTransactionId(paymentDto.Method),
                Status = paymentDto.Status,
                CustomerID = paymentDto.CustomerID
            };

            _context.Payments.Add(payment);

            if (paymentDto.Status == PaymentStatus.Success)
            {
                order.Status = OrderStatus.Delivered;
                _context.Orders.Update(order);
            }

            _context.SaveChanges();
            return payment;
        }

        private string GenerateTransactionId(PaymentMethod method)
        {
            return method switch
            {
                PaymentMethod.Cash => $")}",
                PaymentMethod.UPI => $"",
                PaymentMethod.NetBanking => $"",
                PaymentMethod.Wallet => $"",
                _ => $""
            };
        }

        public Payment GetPaymentByOrderId(int orderId)
        {
            return _context.Payments.FirstOrDefault(p => p.OrderID == orderId);
        }

        public List<AllPaymentsDto> GetAllPayments()
        {
            return _context.Payments
                .Include(p => p.Order)
                .Select(p => new AllPaymentsDto
                {
                    PaymentID = p.PaymentID,
                    OrderID = p.OrderID,
                    Amount = p.Order.TotalAmount,
                    Method = p.Method,
                    Status = p.Status,
                    TransactionID = p.TransactionID,
                    CreatedAt = p.CreatedAt
                }).ToList();
        }

        public async Task<string> CreatePaymentIntent(PaymentDTO paymentDto)
        {
            return paymentDto.Method switch
            {
                PaymentMethod.Card => await _stripeService.CreatePaymentIntent(paymentDto),
                PaymentMethod.UPI => await _upiService.CreateUPIPayment(paymentDto),
                _ => throw new NotSupportedException($"Payment method {paymentDto.Method} not supported")
            };
        }
    }
}