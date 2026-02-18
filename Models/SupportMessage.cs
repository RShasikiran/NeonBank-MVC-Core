namespace NeonBank.Models
{
    public class SupportMessage
    {
        public int Id { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string MessageContent { get; set; } = string.Empty;
        public DateTime SentDate { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;
    }
}
