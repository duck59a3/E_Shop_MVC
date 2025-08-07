using Do_an_II.Hubs;
using Do_an_II.Models.ViewModels;
using Do_an_II.Services.ChatServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Do_an_II.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    [Route("Customer/[controller]/[action]")]
    public class ChatController : Controller 
   { 
        private readonly IChatService _chatService;
        private readonly IHubContext<ChatHub> _hubContext;
        public ChatController(IChatService chatService,IHubContext<ChatHub> hubContext)
        {
            _chatService = chatService;
            _hubContext = hubContext;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            if (userName == null)
            {
                throw new UnauthorizedAccessException("User name is not available.");
            }

            // Lấy hoặc tạo chat room cho customer
            var chatRoom = await _chatService.GetOrCreateChatRoomAsync(userId, userName);
            var messages = await _chatService.GetChatMessagesAsync(chatRoom.Id);

            // Mark messages as read
            await _chatService.MarkMessagesAsReadAsync(chatRoom.Id, userId);

            var viewModel = new ChatVM
            {
                ChatRoomId = chatRoom.Id,
                UserRole = "Customer",
                UserId = userId,
                UserName = userName,
                Messages = messages,
                ChatRoom = chatRoom
            };

            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> GetMessages()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            // Lấy chat room của customer
            var chatRoom = await _chatService.GetOrCreateChatRoomAsync(userId, userName);
            var messages = await _chatService.GetChatMessagesAsync(chatRoom.Id);

            // Mark messages as read
            await _chatService.MarkMessagesAsReadAsync(chatRoom.Id, userId);

            return Json(new
            {
                success = true,
                chatRoomId = chatRoom.Id,
                messages = messages.Select(m => new
                {
                    id = m.Id,
                    senderId = m.SenderId,
                    senderName = m.SenderName,
                    senderRole = m.SenderRole,
                    message = m.Message,
                    sentAt = m.SentAt.ToString("HH:mm dd/MM/yyyy"),
                    isRead = m.IsRead
                })
            });
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageVM model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                // Lấy hoặc tạo chat room cho customer
                var chatRoom = await _chatService.GetOrCreateChatRoomAsync(userId, userName);

                var message = await _chatService.AddMessageAsync(
                    chatRoom.Id, userId, userName, "customer", model.Message);

                var messageData = new
                {
                    id = message.Id,
                    chatRoomId = message.ChatRoomId,
                    senderId = message.SenderId,
                    senderName = message.SenderName,
                    senderRole = message.SenderRole,
                    message = message.Message,
                    sentAt = message.SentAt.ToString("HH:mm dd/MM/yyyy"),
                    isRead = message.IsRead
                };
                await _hubContext.Clients.Group($"ChatRoom_{chatRoom.Id}")
                 .SendAsync("ReceiveMessage", messageData);
                return Json(new { success = true, message = "Tin nhắn đã được gửi" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Có lỗi xảy ra khi gửi tin nhắn {ex}" });
            }
        }

        // Get chat room status
        [HttpGet]
        public async Task<IActionResult> GetChatRoomStatus()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var chatRoom = await _chatService.GetOrCreateChatRoomAsync(userId, userName);
            var unreadCount = await _chatService.GetUnreadMessageCountAsync(chatRoom.Id, userId);

            return Json(new
            {
                success = true,
                chatRoom = new
                {
                    id = chatRoom.Id,
                    status = chatRoom.Status,
                    adminName = chatRoom.AdminName,
                    unreadCount = unreadCount
                }
            });
        }
    }
}
