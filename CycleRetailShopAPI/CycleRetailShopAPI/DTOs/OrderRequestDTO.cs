using System.ComponentModel.DataAnnotations;

namespace CycleRetailShopAPI.Models.DTO
{
    public class OrderRequestDTO
    {
        [Required] public string CustomerName { get; set; }
        [Required] public string CustomerPhone { get; set; }
        [Required, EmailAddress] public string CustomerEmail { get; set; }

        public int? CustomerAddressID { get; set; } // ✅ For existing addresses
        public string? NewCustomerAddress { get; set; } // ✅ For new addresses

        [Required] public int EmployeeID { get; set; }

        [Required]
        public List<OrderDetailDTO> OrderDetails { get; set; }
    }

    public class OrderDetailDTO
    {
        [Required] public int CycleID { get; set; }
        [Required] public int Quantity { get; set; }
    }
}
