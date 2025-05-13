using System.ComponentModel.DataAnnotations;

namespace CycleRetailShopAPI.Models
{
    public enum ReportType { Sales, Stock, BestSellers }
    public class Report
    {
        [Key]
        public int ReportID { get; set; }
        [Required]
        public ReportType Type { get; set; }
        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;
    }
}