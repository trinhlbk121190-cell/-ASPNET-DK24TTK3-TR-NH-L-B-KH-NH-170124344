using System.Threading.Tasks;

namespace MedicalForum.WebApi.Application.Interfaces
{
    public interface INotificationService
    {
        /// <summary>
        /// Sends a direct notification to a specific user.
        /// </summary>
        Task SendNotificationToUserAsync(string userId, string title, string message, string type, string referenceId);

        /// <summary>
        /// Broadcasts an event to all users currently viewing/subscribed to a post.
        /// </summary>
        Task BroadcastToPostGroupAsync(string postId, string eventName, object data);
    }
}
