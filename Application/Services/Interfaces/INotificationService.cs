using Domain.Enums;

namespace Application.Services.Interfaces
{
    public interface INotificationService
    {
        public interface INotificationService
        {
            Task SendNotificationAsync(
                Guid userId,
                string title,
                string message,
                NotificationType type,
                string? actionUrl = null);

            Task MarkAsReadAsync(Guid notificationId);
            Task MarkAllAsReadAsync(Guid userId);
        }
    }
}
