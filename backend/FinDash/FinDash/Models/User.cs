namespace FinDash.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Salt { get; set; } = null!;
        public bool IsAdmin { get; set; }
        public virtual ICollection<UserStock>? UserStocks { get; set; }
    }

}
