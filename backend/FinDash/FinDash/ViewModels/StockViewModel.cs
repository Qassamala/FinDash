namespace FinDash.ViewModels
{
    public class StockViewModel
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = null!;
        public decimal Price { get; set; }
        public DateTime LastUpdated { get; set; }
    }

}
