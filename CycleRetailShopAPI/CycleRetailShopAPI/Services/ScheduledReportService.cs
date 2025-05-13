using System.Threading.Tasks;
using CycleRetailShopAPI.Interfaces;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using CycleRetailShopAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using CycleRetailShopAPI.Models;

public class ScheduledReportService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public ScheduledReportService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SendReportsAsync(string type = "daily") // "daily" or "monthly"
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var reportService = scope.ServiceProvider.GetRequiredService<ISalesReportService>();

            var today = DateTime.Today;
            List<Order> orders;

            if (type == "monthly")
            {
                var startOfMonth = new DateTime(today.Year, today.Month, 1);
                orders = await dbContext.Orders
                    .Include(o => o.Customer)
                    .Where(o => o.OrderDate >= startOfMonth && o.OrderDate <= today)
                    .ToListAsync();
            }
            else
            {
                orders = await dbContext.Orders
                    .Include(o => o.Customer)
                    .Where(o => o.OrderDate.Date == today)
                    .ToListAsync();
            }

            if (orders.Count == 0) return;

            var title = type == "monthly" ? "Monthly Sales Report" : "Daily Sales Report";
            var fileName = type == "monthly" ? "MonthlySalesReport.pdf" : "DailySalesReport.pdf";
            var pdfBytes = reportService.GenerateSalesPdf(orders, title);

            var adminEmails = await dbContext.Users
                .Where(u => u.Role == 0)
                .Select(u => u.Email)
                .ToListAsync();

            foreach (var email in adminEmails)
            {
                SendEmailWithAttachment(email, title, $"Attached is your {title.ToLower()}.", pdfBytes, fileName);
            }
        }
    }


    private void SendEmailWithAttachment(string toEmail, string subject, string body, byte[] attachmentBytes, string attachmentName)
    {
        using var message = new MailMessage();
        message.From = new MailAddress("your-email@gmail.com"); // Update with your sender email
        message.To.Add(toEmail);
        message.Subject = subject;
        message.Body = body;
        message.Attachments.Add(new Attachment(new MemoryStream(attachmentBytes), attachmentName));

        using var smtp = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential("your-email@gmail.com", "your-app-password"), // Use App Password for Gmail
            EnableSsl = true
        };

        smtp.Send(message);
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
