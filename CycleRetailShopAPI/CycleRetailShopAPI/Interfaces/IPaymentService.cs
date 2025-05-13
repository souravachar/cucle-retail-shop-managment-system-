//using CycleRetailShopAPI.DTOs;
//using CycleRetailShopAPI.Models;
//using System.Collections.Generic;

//namespace CycleRetailShopAPI.Interfaces
//{
//    public interface IPaymentService
//    {
//        Payment ProcessPayment(PaymentDTO paymentDto);
//        Payment GetPaymentByOrderId(int orderId);
//        List<AllPaymentsDto> GetAllPayments();

//        // ✅ Stripe Payment Intent
//        Task<string> CreatePaymentIntent(PaymentDTO paymentDto);
//    }
//}


using CycleRetailShopAPI.DTOs;
using CycleRetailShopAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CycleRetailShopAPI.Interfaces
{
    public interface IPaymentService
    {
        Payment ProcessPayment(PaymentDTO paymentDto);
        Payment GetPaymentByOrderId(int orderId);
        List<AllPaymentsDto> GetAllPayments();
        Task<string> CreatePaymentIntent(PaymentDTO paymentDto);
    }
}