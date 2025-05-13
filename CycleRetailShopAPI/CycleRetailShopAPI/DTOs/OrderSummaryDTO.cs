//using CycleRetailShopAPI.Models;

//namespace CycleRetailShopAPI.DTOs
//{
//    public class OrderSummaryDTO
//    {
//        public int OrderID { get; set; }
//        public DateTime OrderDate { get; set; }
//        public decimal TotalAmount { get; set; }
//        public OrderStatus Status { get; set; }
//        public int EmployeeID { get; set; }
//    }
//}

using System.Text.Json.Serialization;
using CycleRetailShopAPI.Models;

namespace CycleRetailShopAPI.DTOs
{
    public class OrderSummaryDTO
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        // ✅ Order Status
        public string Status { get; set; }

        // ✅ Customer Details
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }

        // ✅ Employee Details
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }

        // ✅ Order Details (Cycle Information)
        public List<OrderCycleDetailDTO> OrderDetails { get; set; }
    }

    // ✅ Rename to match the expected type
    public class OrderCycleDetailDTO
    {
        public int CycleID { get; set; }

        [JsonPropertyName("CycleName")]  // ✅ Map 'CycleName' to 'ModelName'
        public string CycleName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }


}

