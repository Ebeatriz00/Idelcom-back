using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Infrastructure.Hubs
{
    [Authorize]
    public class NotificationsHub : Hub 
    {
        public override async Task OnConnectedAsync()
        {
            var bid = Context.User?.FindFirst("bid")?.Value;
            Console.WriteLine($"[WS] Connected user={Context.UserIdentifier} bid={bid}");
            if (!string.IsNullOrEmpty(bid))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"biz:{bid}"); 
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var bid = Context.User?.FindFirst("bid")?.Value;
            if (!string.IsNullOrEmpty(bid))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"biz:{bid}");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}