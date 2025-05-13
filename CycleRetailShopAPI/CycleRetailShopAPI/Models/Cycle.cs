using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CycleRetailShopAPI.Models
{
    public enum CycleType
    {
        Road, Mountain, Hybrid, Electric
    }

    public class Cycle
    {
        [Key]
        public int CycleID { get; set; }

        [Required]
        public string ModelName { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CycleType Type { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }  // ✅ Added Description Field

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }  // ✅ Added UpdatedAt Field
    }
}
