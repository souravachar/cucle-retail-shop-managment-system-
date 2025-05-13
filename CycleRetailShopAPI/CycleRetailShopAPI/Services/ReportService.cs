using CycleRetailShopAPI.Data;
using CycleRetailShopAPI.Interfaces;
using CycleRetailShopAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CycleRetailShopAPI.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<SalesReportDTO> GetSalesReport(DateTime startDate, DateTime endDate)
        {
            return _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new SalesReportDTO
                {
                    Date = g.Key,
                    TotalSales = g.Sum(o => o.TotalAmount),
                    OrdersCount = g.Count()
                })
                .ToList();
        }

        public List<InventoryReportDTO> GetInventoryReport()
        {
            return _context.Cycles
                .Select(c => new InventoryReportDTO
                {
                    CycleID = c.CycleID,
                    CycleName = c.ModelName,
                    StockQuantity = c.StockQuantity
                })
                .ToList();
        }
    }
}
