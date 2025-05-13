using CycleRetailShopAPI.Models;
using CycleRetailShopAPI.DTOs;
using System.Collections.Generic;

namespace CycleRetailShopAPI.Interfaces
{
    public interface IOrderService
    {
        Order PlaceOrder(OrderDTO orderDto);
        List<OrderSummaryDTO> GetOrdersByEmployee(int employeeId); // ✅ Update return type
        List<OrderSummaryDTO> GetAllOrders();



        Task<List<Order>> GetOrdersFromDateAsync(DateTime date);

        Task<List<Order>> GetOrdersFromDateRangeAsync(DateTime startDate, DateTime endDate);




        int GetTotalOrdersCount();

        Order? GetOrderById(int orderId);  // ✅ Add this method

        Order? UpdateOrder(int orderId, OrderUpdateDTO orderUpdateDto);

    }
}
