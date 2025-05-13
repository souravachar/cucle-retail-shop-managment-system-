// File: Services/SalesReportService.cs
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.IO;
using CycleRetailShopAPI.Models;
using CycleRetailShopAPI.Interfaces;

namespace CycleRetailShopAPI.Services
{
    public class SalesReportService : ISalesReportService
    {
        public byte[] GenerateSalesPdf(List<Order> orders, string title)
        {
            using (var stream = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4);
                PdfWriter.GetInstance(doc, stream);
                doc.Open();

                var fontTitle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                var fontHeader = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                var fontBody = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                doc.Add(new Paragraph(title, fontTitle));
                doc.Add(new Paragraph("\n"));

                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;

                table.AddCell(new Phrase("Order ID", fontHeader));
                table.AddCell(new Phrase("Customer", fontHeader));
                table.AddCell(new Phrase("Date", fontHeader));
                table.AddCell(new Phrase("Amount", fontHeader));
                table.AddCell(new Phrase("Status", fontHeader));

                foreach (var order in orders)
                {
                    table.AddCell(new Phrase(order.OrderID.ToString(), fontBody));
                    table.AddCell(new Phrase(order.Customer?.FullName ?? "N/A", fontBody));
                    table.AddCell(new Phrase(order.OrderDate.ToString("dd-MM-yyyy"), fontBody));
                    table.AddCell(new Phrase("₹ " + order.TotalAmount.ToString("0.00"), fontBody));
                    table.AddCell(new Phrase(order.Status.ToString(), fontBody));
                }

                doc.Add(table);
                doc.Close();
                return stream.ToArray();
            }
        }
    }
}
