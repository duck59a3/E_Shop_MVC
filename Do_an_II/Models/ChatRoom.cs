namespace Do_an_II.Models
{
    public class ChatRoom
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string? AdminId { get; set; }
        public string? AdminName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; } = "waiting"; // waiting, active, closed

        public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}

