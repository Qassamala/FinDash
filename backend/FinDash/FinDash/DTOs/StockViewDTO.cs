namespace FinDash.DTOs
{
    public class StockViewDTO
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = null!;
        public decimal Price { get; set; }
        public string CompanyName { get; set; } = null!;
        public string Currency { get; set; } = null!;
        public DateTime LastUpdated { get; set; }
    }

}
