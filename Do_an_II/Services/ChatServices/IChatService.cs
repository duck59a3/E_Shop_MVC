using Do_an_II.Models;

namespace Do_an_II.Services.ChatServices
{
    public interface IChatService
    {
        Task<ChatRoom> GetOrCreateChatRoomAsync(string customerId, string customerName);
        Task<List<ChatRoom>> GetAdminChatRoomsAsync();
        Task<ChatRoom?> GetChatRoomAsync(int chatRoomId);
        Task<List<ChatMessage>> GetChatMessagesAsync(int chatRoomId);
        Task<ChatMessage> AddMessageAsync(int chatRoomId, string senderId, string senderName, string senderRole, string message);
        Task MarkMessagesAsReadAsync(int chatRoomId, string userId);
        Task<bool> CloseChatRoomAsync(int chatRoomId);
        Task<int> GetUnreadMessageCountAsync(int chatRoomId, string userId);
    }
}
