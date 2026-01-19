using Microsoft.AspNetCore.SignalR;

namespace Do_an_II.Hubs
{
    public class NotifyHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            if (Context.User!.IsInRole("Admin"))
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            await base.OnConnectedAsync();
        }
    }
}
