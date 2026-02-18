namespace NeonBank.Models
{
    public class Announcement
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PostedDate { get; set; } = DateTime.Now;
        public string Priority { get; set; } = "Normal";
    }
}
