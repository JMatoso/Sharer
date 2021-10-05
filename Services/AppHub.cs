using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Sharer.Services
{
    public class AppHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SharerHub");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SharerHub");
            await base.OnDisconnectedAsync(exception);
        }
    }
}