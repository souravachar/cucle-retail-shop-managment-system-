using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CycleRetailShopAPI.Interfaces;
using CycleRetailShopAPI.Models;
using CycleRetailShopAPI.Data;
using CycleRetailShopAPI.DTOs;

using Microsoft.EntityFrameworkCore;
using YourProject.DTOs;  // ✅ Added this to fix the ToListAsync() error


namespace CycleRetailShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ApplicationDbContext _context;


        public CustomersController(ICustomerService customerService, ApplicationDbContext context)
        {
            _customerService = customerService;
            _context = context;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _context.Customers
                .Include(c => c.Addresses)  // ✅ Include Addresses
                .ToListAsync();

            var customerData = customers.Select(customer => new
            {
                customerID = customer.CustomerID,
                fullName = customer.FullName,
                phoneNumber = customer.PhoneNumber,
                email = customer.Email,
                addresses = customer.Addresses.Select(a => new
                {
                    addressID = a.AddressID,
                    fullAddress = a.FullAddress
                }).ToList()
            });

            return Ok(customerData);
        }


        // ✅ Update Customer Details
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerUpdateDto updatedCustomerDto)
        {
            if (updatedCustomerDto == null)
                return BadRequest(new { message = "Invalid customer data." });

            var existingCustomer = await _context.Customers
                .Include(c => c.Addresses)
                .FirstOrDefaultAsync(c => c.CustomerID == id);

            if (existingCustomer == null)
                return NotFound(new { message = "Customer not found." });

            // ✅ Update basic details
            existingCustomer.FullName = updatedCustomerDto.FullName;
            existingCustomer.PhoneNumber = updatedCustomerDto.PhoneNumber;
            existingCustomer.Email = updatedCustomerDto.Email;

            // ✅ Update or Add Address Logic
            if (updatedCustomerDto.Addresses != null && updatedCustomerDto.Addresses.Any())
            {
                existingCustomer.Addresses.Clear(); // Clear old addresses to avoid duplicates
                foreach (var addressDto in updatedCustomerDto.Addresses)
                {
                    existingCustomer.Addresses.Add(new CustomerAddress
                    {
                        AddressID = addressDto.AddressID ?? 0, // Retain ID for existing addresses
                        FullAddress = addressDto.FullAddress
                    });
                }
            }

            _context.Customers.Update(existingCustomer);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Customer updated successfully." });  // ✅ Return JSON response
        }





        // ✅ GET Customer by Email
        [HttpGet("email/{email}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> GetCustomerByEmail(string email)
        {
            var customer = await _customerService.GetCustomerByEmail(email);
            if (customer == null)
            {
                return NotFound(new { message = "Customer not found." });
            }

            return Ok(new
            {
                customerID = customer.CustomerID,
                customerName = customer.FullName,
                customerPhone = customer.PhoneNumber,
                customerEmail = customer.Email,
                addresses = customer.Addresses.Select(a => new
                {
                    addressID = a.AddressID,
                    fullAddress = a.FullAddress
                }).ToList()
            });
        }

        // ✅ GET Customer by ID
        [HttpGet("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Addresses)
                .FirstOrDefaultAsync(c => c.CustomerID == id);

            if (customer == null)
            {
                return NotFound(new { message = "Customer not found." });
            }

            return Ok(new
            {
                customerID = customer.CustomerID,
                fullName = customer.FullName,
                phoneNumber = customer.PhoneNumber,
                email = customer.Email,
                addresses = customer.Addresses.Select(a => new
                {
                    addressID = a.AddressID,
                    fullAddress = a.FullAddress
                }).ToList()
            });

        }
    }
}