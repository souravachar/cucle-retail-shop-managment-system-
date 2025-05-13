//using System.ComponentModel.DataAnnotations.Schema;
//using System.ComponentModel.DataAnnotations;

//namespace CycleRetailShopAPI.Models
//{
//    public enum PaymentMethod { Cash, Card, UPI }
//    public enum PaymentStatus { Success, Failed, Pending }

//    public class Payment
//    {
//        [Key]
//        public int PaymentID { get; set; }

//        [Required]
//        public int OrderID { get; set; }
//        [ForeignKey("OrderID")]
//        public Order Order { get; set; }

//        [Required]
//        public PaymentMethod Method { get; set; }

//        public string? TransactionID { get; set; }

//        [Required]
//        public PaymentStatus Status { get; set; }

//        [Required]
//        public int CustomerID { get; set; }
//        [ForeignKey("CustomerID")]
//        public Customer Customer { get; set; }

//        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//    }
//}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CycleRetailShopAPI.Models
{
    public enum PaymentMethod
    {
        Cash = 0,
        Card = 1,
        UPI = 2,
        NetBanking = 3,
        Wallet = 4
    }

    public enum PaymentStatus
    {
        Success = 0,
        Failed = 1,
        Pending = 2
    }

    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }

        [Required]
        public int OrderID { get; set; }
        [ForeignKey("OrderID")]
        public Order Order { get; set; }

        [Required]
        public PaymentMethod Method { get; set; }

        public string? TransactionID { get; set; }

        [Required]
        public PaymentStatus Status { get; set; }

        [Required]
        public int CustomerID { get; set; }
        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}