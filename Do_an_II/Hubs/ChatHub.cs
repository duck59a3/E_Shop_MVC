using Do_an_II.Data;
using Do_an_II.Models;
using Do_an_II.Models.ViewModels;
using Do_an_II.Repository.IRepository;
using Do_an_II.Services.ChatServices;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace Do_an_II.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _db;
        private readonly IChatService _chatService;
        private readonly IUnitOfWork _unitOfWork;
        public ChatHub(ApplicationDbContext db, IChatService chatService, IUnitOfWork unitOfWork)
        {
            _db = db;
            _chatService = chatService;
            _unitOfWork = unitOfWork;
        }
        public override async Task OnConnectedAsync()
        {
            
            var userRole = Context.User?.FindFirst("Role")?.Value.ToLower() ?? "Customer";
            var userId = Context.UserIdentifier;

            if (userRole == "Admin")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "AdminGroup");
            }
            else
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Customer_{userId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            var userRole = Context.User?.FindFirst("Role")?.Value.ToLower() ?? "Customer";

            if (userRole == "Admin")
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AdminGroup");
            }
            else
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Customer_{userId}");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string message, int chatRoomId)
        {
            var userId = Context.UserIdentifier;
            var userName = Context.User?.Identity?.Name;
            var userRole = Context.User?.FindFirst("Role")?.Value ?? "Customer";

            if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(userId))
                return;

            // Lưu tin nhắn vào database
            var chatMessage = new ChatMessage
            {
                ChatRoomId = chatRoomId,
                SenderId = userId,
                SenderName = userName,
                SenderRole = userRole,
                Message = message.Trim(),
                SentAt = DateTime.Now,
                IsRead = false
            };

            
            await _chatService.AddMessageAsync(chatRoomId, userId, userName, userRole, message.Trim());

            // Cập nhật thời gian tin nhắn cuối của room
            var chatRoom = await _db.ChatRooms.FindAsync(chatRoomId);
            if (chatRoom != null)
            {
                chatRoom.LastMessageAt = DateTime.Now;
                //if (chatRoom.Status == "waiting" && userRole == "admin")
                //{
                //    chatRoom.Status = "active";
                //    chatRoom.AdminId = userId;
                //    chatRoom.AdminName = userName;
                //}
            }

            await _db.SaveChangesAsync();

            // Gửi tin nhắn đến các clients
            var messageData = new
            {
                Id = chatMessage.Id,
                ChatRoomId = chatMessage.ChatRoomId,
                SenderId = chatMessage.SenderId,
                SenderName = chatMessage.SenderName,
                SenderRole = chatMessage.SenderRole,
                Message = chatMessage.Message,
                SentAt = chatMessage.SentAt.ToString("HH:mm dd/MM/yyyy"),
                IsRead = chatMessage.IsRead
            };

            await Clients.Group($"ChatRoom_{chatRoomId}").SendAsync("ReceiveMessage", messageData);
        }

        public async Task SendAdminMessage(AdminSendMessageVM model)
        {
            var chatRoom = await _chatService.GetChatRoomAsync(model.ChatRoomId);
            if (chatRoom == null) return;

            // Lưu tin nhắn vào DB
            var chatMessage = await _chatService.AddMessageAsync(
                model.ChatRoomId,
                model.SenderId,
                model.SenderName,
                model.SenderRole,
                model.Message.Trim()
            );

            var messageData = new
            {
                Id = chatMessage.Id,
                ChatRoomId = chatMessage.ChatRoomId,
                SenderId = chatMessage.SenderId,
                SenderName = chatMessage.SenderName,
                SenderRole = chatMessage.SenderRole,
                Message = chatMessage.Message,
                SentAt = chatMessage.SentAt.ToString("HH:mm dd/MM/yyyy"),
                IsRead = chatMessage.IsRead
            };

            // Gửi tới customer đúng phòng
            await Clients.Group($"Customer_{chatRoom.CustomerId}")
                .SendAsync("ReceiveMessage", messageData);

            // Gửi lại cho admin để confirm
            await Clients.Caller.SendAsync("ReceiveMessage", messageData);
        }

        public async Task JoinChatRoom(int chatRoomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"ChatRoom_{chatRoomId}");
        }

        public async Task LeaveChatRoom(int chatRoomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"ChatRoom_{chatRoomId}");
        }



        // Customer gửi tin nhắn cho Admin
        //public async Task SendMessageToAdmin(string message)
        //{
        //    var userId = Context.User.Identity.Name;

        //    // Gửi cho tất cả Admin
        //    await Clients.Group("Admin").SendAsync("ReceiveMessageFromCustomer", userId, message);
        //}

        //// Admin gửi tin nhắn cho Customer
        //public async Task SendMessageToCustomer(string customerId, string message)
        //{
        //    await Clients.Group(customerId).SendAsync("ReceiveMessageFromAdmin", "Admin", message);
        //}
        public async Task MarkMessagesAsRead(int chatRoomId)
        {
            var userId = Context.UserIdentifier;
            var userRole = Context.User?.FindFirst("Role")?.Value ?? "customer";

            var messages = await _db.ChatMessages
                .Where(m => m.ChatRoomId == chatRoomId &&
                           m.SenderId != userId &&
                           !m.IsRead)
                .ToListAsync();

            foreach (var message in messages)
            {
                message.IsRead = true;
            }

            await _db.SaveChangesAsync();

            // Thông báo về việc đã đọc tin nhắn
            await Clients.Group($"ChatRoom_{chatRoomId}").SendAsync("MessagesMarkedAsRead", chatRoomId, userId);
        }

        public async Task AdminTyping(int chatRoomId, bool isTyping)
        {
            var chatRoom = await _db.ChatRooms.FindAsync(chatRoomId);
            if (chatRoom != null)
            {
                await Clients.Group($"Customer_{chatRoom.CustomerId}")
                    .SendAsync("AdminTyping", chatRoomId, isTyping);
            }
        }

        public async Task CustomerTyping(int chatRoomId, bool isTyping)
        {
            await Clients.Group("AdminGroup")
                .SendAsync("CustomerTyping", chatRoomId, isTyping);
        }
    }
}
