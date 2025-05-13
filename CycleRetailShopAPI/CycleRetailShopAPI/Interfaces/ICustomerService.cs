using CycleRetailShopAPI.Models;

namespace CycleRetailShopAPI.Interfaces
{
    public interface ICustomerService
    {
        Task<Customer?> GetCustomerByEmail(string email);  // ✅ New Method
        Task<Customer> GetCustomerById(int id);           // ✅ New method

        Task<Customer> GetOrCreateCustomer(string name, string phone, string email);
        Task<List<CustomerAddress>> GetCustomerAddresses(int customerId);
    }
}
