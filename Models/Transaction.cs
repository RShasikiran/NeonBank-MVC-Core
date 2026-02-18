namespace NeonBank.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public string Description { get; set; } = string.Empty;

        // Navigation property
        public virtual Account? Account { get; set; }
    }
}
