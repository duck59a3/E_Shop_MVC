using Microsoft.AspNetCore.SignalR;

namespace Do_an_II.Hubs
{
    public class OrderTrackingHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            
            await base.OnConnectedAsync();
        }
    }
}
