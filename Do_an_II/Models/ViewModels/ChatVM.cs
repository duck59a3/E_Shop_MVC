namespace Do_an_II.Models.ViewModels
{
    public class ChatVM
    {
        public int ChatRoomId { get; set; }
        public string UserRole { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
        public ChatRoom ChatRoom { get; set; }
    }
}
