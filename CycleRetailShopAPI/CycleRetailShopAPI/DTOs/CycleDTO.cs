namespace CycleRetailShopAPI.Dtos
{
    public class CycleDTO
    {
        public string ModelName { get; set; }
        public string Brand { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? Description { get; set; }  
        public DateTime? UpdatedAt { get; set; }  
    }
}
