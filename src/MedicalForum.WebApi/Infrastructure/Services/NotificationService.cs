using MedicalForum.WebApi.Application.Interfaces;
using MedicalForum.WebApi.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace MedicalForum.WebApi.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationToUserAsync(string userId, string title, string message, string type, string referenceId)
        {
            await _hubContext.Clients.Group(userId).SendAsync("ReceiveNotification", new
            {
                Title = title,
                Message = message,
                Type = type,
                ReferenceId = referenceId
            });
        }

        public async Task BroadcastToPostGroupAsync(string postId, string eventName, object data)
        {
            await _hubContext.Clients.Group($"Post_{postId}").SendAsync(eventName, data);
        }
    }
}
