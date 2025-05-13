using System;

namespace CycleRetailShopAPI.DTOs
{
    public class SalesReportDTO
    {
        public DateTime Date { get; set; }
        public decimal TotalSales { get; set; }
        public int OrdersCount { get; set; }
    }
}
