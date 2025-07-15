using Microsoft.AspNetCore.SignalR;

namespace Do_an_II.Utilities
{
    public class NotificationHub : Hub
    {
       public override async Task OnConnectedAsync()
        {
            var user = Context.User;
            if (user != null && user.IsInRole("Admin"))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            }
            await base.OnConnectedAsync();
        }
    }
}
