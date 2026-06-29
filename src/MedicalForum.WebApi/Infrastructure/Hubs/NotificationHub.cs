using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace MedicalForum.WebApi.Infrastructure.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        // Users join a group named after their UserId to receive private, targeted notifications
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnConnectedAsync();
        }

        // Subscribing to thread updates dynamically
        public async Task SubscribeToPost(string postId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Post_{postId}");
        }

        public async Task UnsubscribeFromPost(string postId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Post_{postId}");
        }
    }
}
