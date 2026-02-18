namespace NeonBank.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public int UserId { get; set; }

        // Navigation property
        public virtual User? User { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
