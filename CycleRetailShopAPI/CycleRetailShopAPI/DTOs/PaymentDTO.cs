using CycleRetailShopAPI.Models;

public class PaymentDTO
{
    public int OrderID { get; set; }
    public PaymentMethod Method { get; set; }
    public string? TransactionID { get; set; }
    public PaymentStatus Status { get; set; }
    public int CustomerID { get; set; } // ✅ Linking payment to a customer
    public decimal Amount { get; set; }  // ✅ Amount for Stripe payment
}
