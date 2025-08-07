using Do_an_II.Models.ViewModels;
using Do_an_II.Services.ChatServices;
using Do_an_II.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Do_an_II.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    [Authorize(Roles = Roles.Admin)]
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.Identity?.Name;

            var chatRooms = await _chatService.GetAdminChatRoomsAsync();

            var viewModel = new AdminChatVM
            {
                ChatRooms = chatRooms,
                AdminId = userId,
                AdminName = userName
            };

            return View(viewModel);
        }

        // Load messages for specific chat room (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetMessages(int chatRoomId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var chatRoom = await _chatService.GetChatRoomAsync(chatRoomId);
            if (chatRoom == null)
            {
                return Json(new { success = false, message = "Không tìm thấy phòng chat" });
            }

            var messages = await _chatService.GetChatMessagesAsync(chatRoomId);

            // Mark messages as read
            await _chatService.MarkMessagesAsReadAsync(chatRoomId, userId);

            return Json(new
            {
                success = true,
                chatRoom = new
                {
                    id = chatRoom.Id,
                    customerId = chatRoom.CustomerId,
                    customerName = chatRoom.CustomerName,
                    adminId = chatRoom.AdminId,
                    adminName = chatRoom.AdminName,
                    status = chatRoom.Status,
                    createdAt = chatRoom.CreatedAt.ToString("HH:mm dd/MM/yyyy")
                },
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

        // Send message (AJAX)
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] AdminSendMessageVM model)
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
                var chatRoom = await _chatService.GetChatRoomAsync(model.ChatRoomId);
                if (chatRoom == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy phòng chat" });
                }

                var message = await _chatService.AddMessageAsync(
                    model.ChatRoomId, userId, userName, "admin", model.Message);

                return Json(new
                {
                    success = true,
                    message = new
                    {
                        id = message.Id,
                        chatRoomId = message.ChatRoomId,
                        senderId = message.SenderId,
                        senderName = message.SenderName,
                        senderRole = message.SenderRole,
                        message = message.Message,
                        sentAt = message.SentAt.ToString("HH:mm dd/MM/yyyy"),
                        isRead = message.IsRead
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi gửi tin nhắn" });
            }
        }

        // Get all chat rooms for admin (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetChatRooms()
        {
            var chatRooms = await _chatService.GetAdminChatRoomsAsync();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = new List<object>();

            foreach (var room in chatRooms)
            {
                var unreadCount = await _chatService.GetUnreadMessageCountAsync(room.Id, userId);
                var lastMessage = room.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault();

                result.Add(new
                {
                    id = room.Id,
                    customerId = room.CustomerId,
                    customerName = room.CustomerName,
                    adminId = room.AdminId,
                    adminName = room.AdminName,
                    createdAt = room.CreatedAt.ToString("HH:mm dd/MM/yyyy"),
                    lastMessageAt = room.LastMessageAt?.ToString("HH:mm dd/MM/yyyy"),
                    status = room.Status,
                    unreadCount = unreadCount,
                    lastMessage = lastMessage != null ? new
                    {
                        message = lastMessage.Message.Length > 50
                            ? lastMessage.Message.Substring(0, 50) + "..."
                            : lastMessage.Message,
                        senderName = lastMessage.SenderName,
                        senderRole = lastMessage.SenderRole,
                        sentAt = lastMessage.SentAt.ToString("HH:mm dd/MM/yyyy")
                    } : null
                });
            }

            return Json(new { success = true, chatRooms = result });
        }

        // Close chat room (Admin only)
        [HttpPost]
        public async Task<IActionResult> CloseChatRoom(int chatRoomId)
        {
            try
            {
                var result = await _chatService.CloseChatRoomAsync(chatRoomId);

                if (result)
                {
                    return Json(new { success = true, message = "Đã đóng phòng chat thành công" });
                }
                else
                {
                    return Json(new { success = false, message = "Không tìm thấy phòng chat" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi đóng phòng chat" });
            }
        }

        // Assign admin to chat room
        [HttpPost]
        public async Task<IActionResult> AssignToRoom(int chatRoomId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.Identity?.Name;

            try
            {
                var chatRoom = await _chatService.GetChatRoomAsync(chatRoomId);
                if (chatRoom == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy phòng chat" });
                }

                
                chatRoom.AdminId = userId;
                chatRoom.AdminName = userName;
                chatRoom.Status = "active";

                
                // await _chatService.AssignAdminToRoomAsync(chatRoomId, userId, userName);

                return Json(new { success = true, message = "Đã nhận phòng chat thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra" });
            }
        }
    }
}
