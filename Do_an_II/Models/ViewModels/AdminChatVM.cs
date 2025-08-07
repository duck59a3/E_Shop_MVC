namespace Do_an_II.Models.ViewModels
{
    public class AdminChatVM
    {
        public List<ChatRoom> ChatRooms { get; set; } = new List<ChatRoom>();
        public int? SelectedChatRoomId { get; set; }
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
        public string AdminId { get; set; }
        public string AdminName { get; set; }
    }
}
