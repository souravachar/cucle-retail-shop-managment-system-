// File: Services/ReportSchedulerService.cs
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using CycleRetailShopAPI.Interfaces;

public class ReportSchedulerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private Timer _timer;

    public ReportSchedulerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(SendReports, null, TimeSpan.Zero, TimeSpan.FromHours(1)); // Runs hourly
        return Task.CompletedTask;
    }

    private async void SendReports(object state)
    {
        using var scope = _serviceProvider.CreateScope();

        var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
        var salesReportService = scope.ServiceProvider.GetRequiredService<ISalesReportService>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

        var now = DateTime.Now;

        if (now.Hour == 0) // midnight
        {
            // ✅ DAILY REPORT
            var dailyOrders = await orderService.GetOrdersFromDateAsync(DateTime.Today.AddDays(-1));
            var dailyPdf = salesReportService.GenerateSalesPdf(dailyOrders, $"Daily Sales Report - {DateTime.Today:dd-MM-yyyy}");

            var admins = await userService.GetAdminEmails();
            await emailService.SendEmailWithAttachmentAsync(admins.ToArray(),
                "Daily Sales Report",
                "Attached is the daily sales report.",
                dailyPdf,
                $"DailySalesReport_{DateTime.Today:ddMMyyyy}.pdf");
        }

        if (now.Day == 1 && now.Hour == 0) // first day of the month
        {
            // ✅ MONTHLY REPORT
            var startDate = new DateTime(now.Year, now.Month - 1, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var monthlyOrders = await orderService.GetOrdersFromDateRangeAsync(startDate, endDate);

            var monthlyPdf = salesReportService.GenerateSalesPdf(monthlyOrders, $"Monthly Sales Report - {startDate:MMMM yyyy}");

            var admins = await userService.GetAdminEmails();
            await emailService.SendEmailWithAttachmentAsync(admins.ToArray(),
                "Monthly Sales Report",
                $"Attached is the monthly sales report for {startDate:MMMM yyyy}.",
                monthlyPdf,
                $"MonthlySalesReport_{startDate:MMMM_yyyy}.pdf");
        }
    }

    public override void Dispose()
    {
        _timer?.Dispose();
        base.Dispose();
    }
}
