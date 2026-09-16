using Domain.Enums;

namespace Application.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(
            Guid recipientUserId,
            Guid? actorUserId,
            string? actorName,
            string? actorAvatarUrl,
            string title,
            string message,
            NotificationType type,
            NotificationSourceEntityType? sourceEntityType,
            Guid? sourceEntityId,
            string? actionUrl,
            string createdBy);

        Task MarkAsReadAsync(Guid notificationId, Guid userId);
        Task MarkAllAsReadAsync(Guid userId);
        Task MarkAsReadBySourceAsync(Guid userId, NotificationSourceEntityType sourceEntityType, Guid sourceEntityId);
    }
}