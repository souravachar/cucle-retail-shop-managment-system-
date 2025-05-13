using CycleRetailShopAPI.DTOs;
using System.Threading.Tasks;

namespace CycleRetailShopAPI.Interfaces
{
    public interface IStripeService
    {
        Task<string> CreatePaymentIntent(PaymentDTO paymentDto);
    }
}
