// File: Interfaces/ISalesReportService.cs
using System.Collections.Generic;
using CycleRetailShopAPI.Models;

namespace CycleRetailShopAPI.Interfaces
{
    public interface ISalesReportService
    {
        byte[] GenerateSalesPdf(List<Order> orders, string title);
    }
}
