using CycleRetailShopAPI.Interfaces;
using CycleRetailShopAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using CycleRetailShopAPI.Services;

namespace CycleRetailShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IStripeService _stripeService;

        public PaymentController(IPaymentService paymentService, IStripeService stripeService)
        {
            _paymentService = paymentService;
            _stripeService = stripeService;
        }


        [HttpPost("create-payment-intent")]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] PaymentDTO paymentDto)
        {
            try
            {
                var clientSecret = await _stripeService.CreatePaymentIntent(paymentDto);
                return Ok(new { clientSecret });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("process")]
        public IActionResult ProcessPayment([FromBody] PaymentDTO paymentDto)
        {
            try
            {
                var payment = _paymentService.ProcessPayment(paymentDto);
                return Ok(payment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("payment-by-orderID/{orderId}")]
        public IActionResult GetPaymentByOrderId(int orderId)
        {
            var payment = _paymentService.GetPaymentByOrderId(orderId);
            if (payment == null)
                return NotFound(new { message = "Payment not found for this order." });

            return Ok(payment);
        }

        
        [HttpGet("get-all-payments")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllPayments()
        {
            var payments = _paymentService.GetAllPayments();
            return Ok(payments);
        }
    }
}
