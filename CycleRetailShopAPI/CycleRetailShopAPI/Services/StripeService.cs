using CycleRetailShopAPI.DTOs;
using Stripe;
using Stripe.Checkout;
using System.Threading.Tasks;

namespace CycleRetailShopAPI.Interfaces
{
    public class StripeService : IStripeService
    {
        public async Task<string> CreatePaymentIntent(PaymentDTO paymentDto)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(paymentDto.Amount * 100), // Amount in cents
                Currency = "inr",
                PaymentMethodTypes = new List<string> { "card" },
                Metadata = new Dictionary<string, string>
                {
                    { "OrderID", paymentDto.OrderID.ToString() },
                    { "CustomerID", paymentDto.CustomerID.ToString() }
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            return paymentIntent.ClientSecret; 
        }
    }
}
