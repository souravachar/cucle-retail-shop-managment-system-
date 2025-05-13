using CycleRetailShopAPI.Interfaces;
using CycleRetailShopAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CycleRetailShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IOrderService _orderService;
        private readonly ISalesReportService _salesReportService;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;

        public ReportController(
            IReportService reportService,
            IOrderService orderService,
            ISalesReportService salesReportService,
            IEmailService emailService,
            IUserService userService)
        {
            _reportService = reportService;
            _orderService = orderService;
            _salesReportService = salesReportService;
            _emailService = emailService;
            _userService = userService;
        }

        [HttpGet("sales/{startDate}/{endDate}")]
        public IActionResult GetSalesReport(DateTime startDate, DateTime endDate)
        {
            var report = _reportService.GetSalesReport(startDate, endDate);
            return Ok(report);
        }

        [HttpGet("inventory")]
        public IActionResult GetInventoryReport()
        {
            var report = _reportService.GetInventoryReport();
            return Ok(report);
        }

        [HttpGet("send-daily-sales-report")]
        public async Task<IActionResult> SendDailyReport()
        {
            var orders = await _orderService.GetOrdersFromDateAsync(DateTime.UtcNow.Date);
            var pdf = _salesReportService.GenerateSalesPdf(orders, "Daily Sales Report - " + DateTime.Today.ToShortDateString());

            var admins = await _userService.GetAdminEmails(); // ✅ Returns List<string>
            await _emailService.SendEmailWithAttachmentAsync(admins.ToArray(),
                "Daily Sales Report",
                "Please find attached the daily sales report.",
                pdf,
                "DailySalesReport.pdf");

            return Ok("Email sent successfully.");
        }
    }
}
