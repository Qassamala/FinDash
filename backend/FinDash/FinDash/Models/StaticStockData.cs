namespace FinDash.Models
{
    public class StaticStockData
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string Currency { get; set; } = null!;
        public virtual ICollection<StockPrice>? StockPrices { get; set; }
        public virtual ICollection<UserStock>? UserStocks { get; set; }
    }
}
