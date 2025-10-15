using Do_an_II.Data;
using Do_an_II.Models;
using Microsoft.EntityFrameworkCore;

namespace Do_an_II.Services.ChatServices
{
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _context;

        public ChatService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ChatMessage> AddMessageAsync(int chatRoomId, string senderId, string senderName, string senderRole, string message)
        {
            var chatMessage = new ChatMessage
            {
                ChatRoomId = chatRoomId,
                SenderId = senderId,
                SenderName = senderName,
                SenderRole = senderRole,
                Message = message,
                SentAt = DateTime.Now,
                IsRead = false
            };

            _context.ChatMessages.Add(chatMessage);

            // Cập nhật thời gian tin nhắn cuối
            var chatRoom = await _context.ChatRooms.FindAsync(chatRoomId);
            if (chatRoom != null)
            {
                chatRoom.LastMessageAt = DateTime.Now;
                if (chatRoom.Status == "waiting" && senderRole.ToLower() == "admin")
                {
                    chatRoom.Status = "active";
                    chatRoom.AdminId = senderId;
                    chatRoom.AdminName = senderName;
                }
            }

            await _context.SaveChangesAsync();
            return chatMessage;
        }

        public async Task<bool> CloseChatRoomAsync(int chatRoomId)
        {
            var chatRoom = await _context.ChatRooms.FindAsync(chatRoomId);
            if (chatRoom != null)
            {
                chatRoom.IsActive = false;
                chatRoom.Status = "closed";
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<ChatRoom>> GetAdminChatRoomsAsync()
        {
            return await _context.ChatRooms
                .Include(r => r.Messages.OrderByDescending(m => m.SentAt).Take(1))
                .Where(r => r.IsActive)
                .OrderByDescending(r => r.LastMessageAt ?? r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ChatMessage>> GetChatMessagesAsync(int chatRoomId)
        {
            return await _context.ChatMessages
                .Where(m => m.ChatRoomId == chatRoomId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<ChatRoom?> GetChatRoomAsync(int chatRoomId)
        {
            return await _context.ChatRooms
                .Include(r => r.Messages)
                .FirstOrDefaultAsync(r => r.Id == chatRoomId);
        }

        public async Task<ChatRoom> GetOrCreateChatRoomAsync(string customerId, string customerName)
        {
            var existingRoom = await _context.ChatRooms
                .Where(r => r.CustomerId == customerId && r.IsActive)
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync();

            if (existingRoom != null)
            {
                return existingRoom;
            }

            // Tạo chat room mới
            var newRoom = new ChatRoom
            {
                CustomerId = customerId,
                CustomerName = customerName,
                CreatedAt = DateTime.Now,
                IsActive = true,
                Status = "waiting"
            };

            _context.ChatRooms.Add(newRoom);
            await _context.SaveChangesAsync();

            return newRoom;
        }

        public async Task<int> GetUnreadMessageCountAsync(int chatRoomId, string userId)
        {
            return await _context.ChatMessages
                .CountAsync(m => m.ChatRoomId == chatRoomId && m.SenderId != userId && !m.IsRead);
        }

        public async Task MarkMessagesAsReadAsync(int chatRoomId, string userId)
        {
            var messages = await _context.ChatMessages
                .Where(m => m.ChatRoomId == chatRoomId && m.SenderId != userId && !m.IsRead)
                .ToListAsync();

            foreach (var message in messages)
            {
                message.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}
