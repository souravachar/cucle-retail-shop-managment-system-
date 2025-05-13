using CycleRetailShopAPI.Models.DTO;

public class OrderDTO
{
    public string CustomerName { get; set; }
    public string CustomerPhone { get; set; }
    public string CustomerEmail { get; set; }
    public string? NewCustomerAddress { get; set; }  // ✅ New field for dynamic address creation
    public int EmployeeID { get; set; }
    public List<OrderDetailDTO> OrderDetails { get; set; }
}
