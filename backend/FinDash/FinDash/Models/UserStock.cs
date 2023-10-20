using System.ComponentModel.DataAnnotations;

namespace FinDash.Models
{
    public class UserStock
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public int StaticStockDataId { get; set; }
        public StaticStockData StaticStockData { get; set; }
    }

}
