using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CycleRetailShopAPI.Models
{
    public class CustomerAddress
    {
        [Key]
        public int AddressID { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [Required, MaxLength(300)]
        public string FullAddress { get; set; }

        public DateTime CreatedAt { get; set; }  // ✅ Added CreatedAt property

        // ✅ Navigation Property
        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; }
    }
}
