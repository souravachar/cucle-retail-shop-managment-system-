using CycleRetailShopAPI.Models;

public class AllPaymentsDto
{
    public int PaymentID { get; set; }
    public int OrderID { get; set; }
    public decimal Amount { get; set; }  // ✅ Add Amount field
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public string? TransactionID { get; set; }
    public DateTime CreatedAt { get; set; }
}
