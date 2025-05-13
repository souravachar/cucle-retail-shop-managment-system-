//using System.ComponentModel.DataAnnotations.Schema;
//using System.ComponentModel.DataAnnotations;

//namespace CycleRetailShopAPI.Models
//{
//    public enum OrderStatus { Pending, Delivered, Cancled }
//    public class Order
//    {
//        [Key]
//        public int OrderID { get; set; }
//        [Required]
//        public int CustomerID { get; set; }
//        [ForeignKey("CustomerID")]
//        public Customer Customer { get; set; }
//        [Required]
//        public int EmployeeID { get; set; }
//        [ForeignKey("EmployeeID")]
//        public User Employee { get; set; }
//        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
//        [Required]
//        public decimal TotalAmount { get; set; }
//        [Required]
//        public OrderStatus Status { get; set; }
//    }
//}


using CycleRetailShopAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CycleRetailShopAPI.Models
{
    public enum OrderStatus { Pending, Delivered, Cancled }
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        [Required]
        public int CustomerID { get; set; }
        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; }

        [Required]
        public int AddressID { get; set; }  // 🔥 New Field for Address Mapping
        [ForeignKey("AddressID")]
        public CustomerAddress Address { get; set; }

        [Required]
        public int EmployeeID { get; set; }
        [ForeignKey("EmployeeID")]
        public User Employee { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    }

}