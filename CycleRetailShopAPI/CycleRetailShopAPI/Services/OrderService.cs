using CycleRetailShopAPI.Data;
using CycleRetailShopAPI.Interfaces;
using CycleRetailShopAPI.Models;
using CycleRetailShopAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CycleRetailShopAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICustomerService _customerService;

        public OrderService(ApplicationDbContext context, ICustomerService customerService)
        {
            _context = context;
            _customerService = customerService; // ✅ Ensure this is injected
        }

        public Order PlaceOrder(OrderDTO orderDto)
        {
            if (orderDto == null || orderDto.OrderDetails == null || !orderDto.OrderDetails.Any())
            {
                throw new ArgumentException("Invalid order data.");
            }

            var customer = _customerService.GetOrCreateCustomer(
                               orderDto.CustomerName,
                               orderDto.CustomerPhone,
                               orderDto.CustomerEmail
                           ).Result;

            // ✅ Handle Existing Customer or Create New
            if (customer == null)
            {
                customer = new Customer
                {
                    FullName = orderDto.CustomerName,
                    PhoneNumber = orderDto.CustomerPhone,
                    Email = orderDto.CustomerEmail,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Customers.Add(customer);
                _context.SaveChanges();
            }

            // ✅ Check if Address Exists for the Customer
            var selectedAddress = _context.CustomerAddresses
                .FirstOrDefault(a => a.CustomerID == customer.CustomerID && a.FullAddress == orderDto.NewCustomerAddress);

            // ✅ Create New Address Only if Not Found
            if (selectedAddress == null)
            {
                selectedAddress = new CustomerAddress
                {
                    CustomerID = customer.CustomerID,
                    FullAddress = orderDto.NewCustomerAddress,
                    CreatedAt = DateTime.UtcNow
                };

                _context.CustomerAddresses.Add(selectedAddress);
                _context.SaveChanges();
            }

            decimal totalAmount = 0;

            var order = new Order
            {
                CustomerID = customer.CustomerID,
                AddressID = selectedAddress.AddressID, // ✅ Now correctly references the found or newly created address
                EmployeeID = orderDto.EmployeeID,
                Status = OrderStatus.Pending,
                OrderDate = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (var item in orderDto.OrderDetails)
            {
                var cycle = _context.Cycles.FirstOrDefault(c => c.CycleID == item.CycleID);

                if (cycle != null && cycle.StockQuantity >= item.Quantity)
                {
                    cycle.StockQuantity -= item.Quantity;
                    decimal itemTotalPrice = cycle.Price * item.Quantity;

                    totalAmount += itemTotalPrice;

                    var orderDetail = new OrderDetail
                    {
                        OrderID = order.OrderID,
                        CycleID = item.CycleID,
                        Quantity = item.Quantity,
                        UnitPrice = cycle.Price
                    };
                    _context.OrderDetails.Add(orderDetail);
                }
            }

            order.TotalAmount = totalAmount;
            _context.SaveChanges();

            return order;
        }



        public List<OrderSummaryDTO> GetOrdersByEmployee(int employeeId)
        {
            return _context.Orders
                .Where(o => o.EmployeeID == employeeId)
                .Include(o => o.Customer)            // Customer Details
                .Include(o => o.Address)             // Customer Address
                .Include(o => o.Employee)            // Employee Details
                .Include(o => o.OrderDetails)        // Order Details
                    .ThenInclude(od => od.Cycle)     // Cycle Details inside Order Details
                .Select(o => new OrderSummaryDTO
                {
                    OrderID = o.OrderID,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status.ToString(),

                    // ✅ Customer Details
                    CustomerID = o.CustomerID,
                    CustomerName = o.Customer.FullName,
                    CustomerPhone = o.Customer.PhoneNumber,
                    CustomerEmail = o.Customer.Email,
                    CustomerAddress = o.Address.FullAddress,

                    // ✅ Employee Details
                    EmployeeID = o.EmployeeID,
                    EmployeeName = o.Employee.Username,

                    OrderDetails = o.OrderDetails.Select(od => new OrderCycleDetailDTO
                    {
                        CycleID = od.CycleID,
                        CycleName = od.Cycle.ModelName,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice
                    }).ToList()

                })
                .ToList();
        }


        public List<OrderSummaryDTO> GetAllOrders()
        {
            return _context.Orders
                .Include(o => o.Customer)            // Customer Details
                .Include(o => o.Address)             // Customer Address
                .Include(o => o.Employee)            // Employee Details
                .Include(o => o.OrderDetails)        // Order Details
                    .ThenInclude(od => od.Cycle)     // Cycle Details inside Order Details
                .Select(o => new OrderSummaryDTO
                {
                    OrderID = o.OrderID,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status.ToString(),

                    // ✅ Customer Details
                    CustomerID = o.CustomerID,
                    CustomerName = o.Customer.FullName,
                    CustomerPhone = o.Customer.PhoneNumber,
                    CustomerEmail = o.Customer.Email,
                    CustomerAddress = o.Address.FullAddress,

                    // ✅ Employee Details
                    EmployeeID = o.EmployeeID,
                    EmployeeName = o.Employee.Username,

                    OrderDetails = o.OrderDetails.Select(od => new OrderCycleDetailDTO
                    {
                        CycleID = od.CycleID,
                        CycleName = od.Cycle.ModelName,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice
                    }).ToList()



                })
                .ToList();
        }



        public Order? UpdateOrder(int orderId, OrderUpdateDTO orderUpdateDto)
        {
            var existingOrder = _context.Orders
                .Include(o => o.Address)
                .FirstOrDefault(o => o.OrderID == orderId);

            if (existingOrder == null)
            {
                throw new Exception("Order not found.");
            }

            // ✅ Update Order Status
            if (orderUpdateDto.Status.HasValue)
            {
                if (!Enum.IsDefined(typeof(OrderStatus), orderUpdateDto.Status.Value))
                {
                    throw new Exception("Invalid order status value.");
                }
                existingOrder.Status = (OrderStatus)orderUpdateDto.Status.Value;
            }

            // ✅ Update Address if Provided
            if (!string.IsNullOrEmpty(orderUpdateDto.NewAddress))
            {
                var existingAddress = _context.CustomerAddresses
                    .FirstOrDefault(a => a.CustomerID == existingOrder.CustomerID && a.FullAddress == orderUpdateDto.NewAddress);

                if (existingAddress == null)
                {
                    var newAddress = new CustomerAddress
                    {
                        CustomerID = existingOrder.CustomerID,
                        FullAddress = orderUpdateDto.NewAddress,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.CustomerAddresses.Add(newAddress);
                    _context.SaveChanges();

                    existingOrder.AddressID = newAddress.AddressID;
                }
                else
                {
                    existingOrder.AddressID = existingAddress.AddressID;
                }
            }

            _context.SaveChanges();

            return existingOrder;
        }

        public int GetTotalOrdersCount()
        {
            return _context.Orders.Count();
        }

        public Order? GetOrderById(int orderId)
        {
            return _context.Orders.FirstOrDefault(order => order.OrderID == orderId);
        }

        public async Task<List<Order>> GetOrdersFromDateAsync(DateTime date)
        {
            var utcDate = date.Kind == DateTimeKind.Utc ? date : date.ToUniversalTime();

            return await _context.Orders
                .Where(o => o.OrderDate.Date == utcDate.Date)
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.Address)
                .Include(o => o.OrderDetails)
                .ToListAsync();
        }


        public async Task<List<Order>> GetFullOrdersFromDateAsync(DateTime date)
        {
            return await _context.Orders
                .Where(o => o.OrderDate.Date == date.Date)
                .Include(o => o.Customer)
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersFromDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .Where(o => o.OrderDate.Date >= startDate.Date && o.OrderDate.Date <= endDate.Date)
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.Address)
                .Include(o => o.OrderDetails)
                .ToListAsync();
        }




    }
}
