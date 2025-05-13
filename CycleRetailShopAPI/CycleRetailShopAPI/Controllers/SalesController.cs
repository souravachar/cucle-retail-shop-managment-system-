using CycleRetailShopAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class SalesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SalesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 📌 Fetch Sales by Category
    [HttpGet("sales-by-category")]
    public IActionResult GetSalesByCategory()
    {
        var categorySales = _context.OrderDetails
            .Include(od => od.Cycle)
            .GroupBy(od => od.Cycle.Type) // 🔥 Use Type instead of Category
            .Select(g => new
            {
                Name = g.Key.ToString(), // Convert Enum to String
                Sales = g.Sum(od => od.Quantity * od.UnitPrice)
            })
            .ToList();

        return Ok(categorySales);
    }


    // 📌 Fetch Sales by Month
    [HttpGet("sales-by-month")]
    public IActionResult GetSalesByMonth()
    {
        var salesByMonth = _context.OrderDetails
            .Include(od => od.Order)
            .GroupBy(od => od.Order.OrderDate.Month)
            .Select(g => new
            {
                Month = g.Key,
                Revenue = g.Sum(od => od.Quantity * od.UnitPrice)
            })
            .OrderBy(s => s.Month)
            .ToList();

        return Ok(salesByMonth);
    }


    // 📌 Fetch Top 3 Cycles
    [HttpGet("top-cycles")]
    public IActionResult GetTopThreeCycles()
    {
        var topCycles = _context.OrderDetails
            .Include(od => od.Cycle)
            .GroupBy(od => od.Cycle.ModelName) // 🔥 Use ModelName instead of Name
            .Select(g => new
            {
                Name = g.Key,
                Sales = g.Sum(od => od.Quantity)
            })
            .OrderByDescending(p => p.Sales)
            .Take(3)
            .ToList();

        return Ok(topCycles);
    }

}
