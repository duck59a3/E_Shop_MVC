using Microsoft.AspNetCore.SignalR;

namespace Do_an_II.Hubs
{
    public class DashboardHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("Connected", "Dashboard connected successfully");
        }
    }
}
