using CycleRetailShopAPI.DTOs;
using System.Threading.Tasks;

namespace CycleRetailShopAPI.Interfaces
{
    public interface IUPIService
    {
        Task<string> CreateUPIPayment(PaymentDTO paymentDto);
    }
}