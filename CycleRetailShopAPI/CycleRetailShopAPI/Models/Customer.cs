//using System.ComponentModel.DataAnnotations;

//namespace CycleRetailShopAPI.Models
//{
//    public class Customer
//    {
//        [Key]
//        public int CustomerID { get; set; }
//        [Required, MaxLength(100)]
//        public string FullName { get; set; }
//        [Required]
//        public string PhoneNumber { get; set; }
//        [Required, EmailAddress]
//        public string Email { get; set; }
//        public string Address { get; set; }
//        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//    }
//}

using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CycleRetailShopAPI.Models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ✅ New Navigation Property for Addresses
        public ICollection<CustomerAddress> Addresses { get; set; }
    }
}
