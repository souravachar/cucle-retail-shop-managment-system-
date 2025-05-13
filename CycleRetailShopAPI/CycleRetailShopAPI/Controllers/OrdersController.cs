using CycleRetailShopAPI.Interfaces;
using CycleRetailShopAPI.Models;
using CycleRetailShopAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using Newtonsoft.Json;

[Route("api/orders")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("place")]
    [Authorize(Roles = "Employee,Admin")]
    public IActionResult PlaceOrder([FromBody] OrderDTO orderDto)
    {

        Console.WriteLine("🟡 Received OrderDTO: " + JsonConvert.SerializeObject(orderDto));


        if (orderDto.OrderDetails == null || !orderDto.OrderDetails.Any())
        {
            Console.WriteLine("❌ Order Details are empty or missing.");
            return BadRequest(new { message = "Order Details cannot be empty." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid order data. Please check all required fields.");
        }

        try
        {
            var order = _orderService.PlaceOrder(orderDto);
            return Ok(new
            {
                message = "Order placed successfully!",
                OrderID = order.OrderID,
                CustomerID = order.CustomerID,
                AddressID = order.AddressID
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to place order.", error = ex.Message });
        }
    }

    [HttpGet("my-orders")]
    [Authorize(Roles = "Employee")]
    public IActionResult GetOrdersByEmployee()
    {
        var employeeId = int.Parse(User.FindFirst("id")?.Value);
        var orders = _orderService.GetOrdersByEmployee(employeeId);
        return Ok(orders);
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetAllOrders()
    {
        var orders = _orderService.GetAllOrders();
        return Ok(orders);
    }


    [HttpPut("update/{orderId}")]
    [Authorize(Roles = "Admin,Employee")]
    public IActionResult UpdateOrder(int orderId, [FromBody] OrderUpdateDTO orderUpdateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid data provided.");
        }

        try
        {
            var updatedOrder = _orderService.UpdateOrder(orderId, orderUpdateDto);
            if (updatedOrder == null)
            {
                return NotFound(new { message = "Order not found." });
            }

            return Ok(new
            {
                message = "Order updated successfully.",
                OrderID = updatedOrder.OrderID,
                Status = updatedOrder.Status.ToString(),
                Address = updatedOrder.Address.FullAddress
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to update order.", error = ex.Message });
        }
    }

    [HttpGet("count")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetTotalOrdersCount()
    {
        var totalOrders = _orderService.GetTotalOrdersCount();
        return Ok(new { totalOrders });
    }

}
