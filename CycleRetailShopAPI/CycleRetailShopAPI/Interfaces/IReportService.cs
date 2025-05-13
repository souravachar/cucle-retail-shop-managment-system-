using CycleRetailShopAPI.DTOs;
using System;
using System.Collections.Generic;

namespace CycleRetailShopAPI.Interfaces
{
    public interface IReportService
    {
        List<SalesReportDTO> GetSalesReport(DateTime startDate, DateTime endDate);
        List<InventoryReportDTO> GetInventoryReport();
    }
}
