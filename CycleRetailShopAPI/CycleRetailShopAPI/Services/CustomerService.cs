using CycleRetailShopAPI.Data;
using CycleRetailShopAPI.Interfaces;
using CycleRetailShopAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CycleRetailShopAPI.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Get Customer by Email
        public async Task<Customer?> GetCustomerByEmail(string email)
        {
            return await _context.Customers
                                 .Include(c => c.Addresses)
                                 .FirstOrDefaultAsync(c => c.Email == email);
        }

     

        public async Task<Customer> GetCustomerById(int id)  // ✅ New method
        {
            return await _context.Customers
                .Include(c => c.Addresses)
                .FirstOrDefaultAsync(c => c.CustomerID == id);
        }

        public async Task<Customer> GetOrCreateCustomer(string name, string phone, string email)
        {
            var customer = await GetCustomerByEmail(email);

            if (customer == null)
            {
                customer = new Customer
                {
                    FullName = name,
                    PhoneNumber = phone,
                    Email = email,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
            }

            return customer;
        }

        public async Task<List<CustomerAddress>> GetCustomerAddresses(int customerId)
        {
            return await _context.CustomerAddresses
                                 .Where(a => a.CustomerID == customerId)
                                 .ToListAsync();
        }
    }
}
