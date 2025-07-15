using Microsoft.AspNetCore.SignalR;

namespace Do_an_II.Hubs
{
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var userId = Context.User.Identity.Name;

            if (httpContext.Request.Query["isAdmin"] == "true")
            {
                // Nếu là admin → thêm vào nhóm Admin
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admin");
            }
            else
            {
                // Nếu là customer → thêm vào nhóm theo userId
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnConnectedAsync();
        }

        // Customer gửi tin nhắn cho Admin
        public async Task SendMessageToAdmin(string message)
        {
            var userId = Context.User.Identity.Name;

            // Gửi cho tất cả Admin
            await Clients.Group("Admin").SendAsync("ReceiveMessageFromCustomer", userId, message);
        }

        // Admin gửi tin nhắn cho Customer
        public async Task SendMessageToCustomer(string customerId, string message)
        {
            await Clients.Group(customerId).SendAsync("ReceiveMessageFromAdmin", "Admin", message);
        }
    }
}
