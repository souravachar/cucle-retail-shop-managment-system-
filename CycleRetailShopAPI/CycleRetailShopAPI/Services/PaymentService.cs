//using CycleRetailShopAPI.Data;
//using CycleRetailShopAPI.Interfaces;
//using CycleRetailShopAPI.Models;
//using CycleRetailShopAPI.DTOs;
//using Microsoft.EntityFrameworkCore;
//using Stripe;
//using Stripe.Checkout;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace CycleRetailShopAPI.Services
//{
//    public class PaymentService : IPaymentService
//    {
//        private readonly ApplicationDbContext _context;

//        public PaymentService(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public Payment ProcessPayment(PaymentDTO paymentDto)
//        {
//            var order = _context.Orders.FirstOrDefault(o => o.OrderID == paymentDto.OrderID);
//            if (order == null)
//                throw new KeyNotFoundException("Order not found");

//            var payment = new Payment
//            {
//                OrderID = paymentDto.OrderID,
//                Method = paymentDto.Method,
//                TransactionID = paymentDto.TransactionID ?? $"CASH-{new Random().Next(1000, 9999)}", 
//                Status = paymentDto.Status,
//                CustomerID = paymentDto.CustomerID
//            };

//            _context.Payments.Add(payment);

//            if (paymentDto.Status == PaymentStatus.Success)
//            {
//                order.Status = OrderStatus.Delivered;
//                _context.Orders.Update(order);
//            }

//            _context.SaveChanges();
//            return payment;
//        }

//        public Payment GetPaymentByOrderId(int orderId)
//        {
//            return _context.Payments.FirstOrDefault(p => p.OrderID == orderId);
//        }

//        public List<AllPaymentsDto> GetAllPayments()
//        {
//            return _context.Payments
//                .Include(p => p.Order)  // Ensure Order is included
//                .Select(p => new AllPaymentsDto
//                {
//                    PaymentID = p.PaymentID,
//                    OrderID = p.OrderID,
//                    Amount = p.Order.TotalAmount,  // ✅ Fetching amount from Order
//                    Method = p.Method,
//                    Status = p.Status,
//                    TransactionID = p.TransactionID,
//                    CreatedAt = p.CreatedAt
//                }).ToList();
//        }


//        public async Task<string> CreatePaymentIntent(PaymentDTO paymentDto)
//        {
//            StripeConfiguration.ApiKey = "sk_test_51QzGEbCvKwexflVfigKPCBFhGuDklUa1TFP4NwZlpOBBOH9r1wWx1fAYTpnKwHzCKgCQLnQycFqGk2ukWpFxxjKb00T8VBY2gZ";
//            var options = new PaymentIntentCreateOptions
//            {
//                Amount = (long)(paymentDto.Amount * 100), // Convert to cents
//                Currency = "inr",
//                PaymentMethodTypes = new List<string> { "card" },
//                Metadata = new Dictionary<string, string>
//                {
//                    { "OrderID", paymentDto.OrderID.ToString() },
//                    { "CustomerID", paymentDto.CustomerID.ToString() }
//                }
//            };

//            var service = new PaymentIntentService();
//            var paymentIntent = await service.CreateAsync(options);

//            return paymentIntent.ClientSecret; // Transaction ID
//        }
//    }
//}

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
                PaymentMethod.Cash => $"CASH-{new Random().Next(1000, 9999)}",
                PaymentMethod.UPI => $"UPI-{DateTime.Now:yyyyMMddHHmmss}",
                PaymentMethod.NetBanking => $"NB-{DateTime.Now:yyyyMMddHHmmss}",
                PaymentMethod.Wallet => $"WALLET-{DateTime.Now:yyyyMMddHHmmss}",
                _ => $"TXN-{DateTime.Now:yyyyMMddHHmmss}"
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