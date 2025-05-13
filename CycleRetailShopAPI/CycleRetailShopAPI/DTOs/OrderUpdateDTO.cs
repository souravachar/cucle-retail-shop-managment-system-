using System.ComponentModel.DataAnnotations;

namespace CycleRetailShopAPI.DTOs
{
    public class OrderUpdateDTO
    {
        [Required]
        public int OrderID { get; set; }

        [Required]
        public int? Status { get; set; } // 🔄 Enum values as int (Pending = 0, Delivered = 1, Cancelled = 2)

        public string? NewAddress { get; set; } // Optional address update
    }
}
