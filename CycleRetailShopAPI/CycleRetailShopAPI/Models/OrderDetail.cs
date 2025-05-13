using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CycleRetailShopAPI.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailID { get; set; }
        [Required]
        public int OrderID { get; set; }
        [ForeignKey("OrderID")]
        public Order Order { get; set; }
        [Required]
        public int CycleID { get; set; }
        [ForeignKey("CycleID")]
        public Cycle Cycle { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }
    }
}