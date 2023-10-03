namespace FinDash.Models
{
    public class StockPrice
    {
        public int Id { get; set; }
        public int StaticStockDataId { get; set; } // Foreign Key to StaticStockData
        public DateTime Timestamp { get; set; }
        public decimal Price { get; set; }

        // Navigation property to StaticStockData
        public virtual StaticStockData StaticStockData { get; set; } = null!;
    }

}
