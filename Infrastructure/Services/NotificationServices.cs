using Application.Common.Repositories;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Services
{
    public class NotificationService(
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork,
        IHubContext<NotificationHub> hubContext) : INotificationService
    {
        public async Task SendNotificationAsync(
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
            string createdBy)
        {
            var notification = new Notification
            {
                UserId = recipientUserId,
                ActorUserId = actorUserId,
                ActorName = actorName,
                ActorAvatarUrl = actorAvatarUrl,
                Title = title,
                Message = message,
                Type = type,
                SourceEntityType = sourceEntityType,
                SourceEntityId = sourceEntityId,
                ActionUrl = actionUrl,
                CreatedBy = createdBy
            };

            await notificationRepository.AddAsync(notification);
            await unitOfWork.SaveAsync();

            var unreadCount = await notificationRepository.GetUnreadCountAsync(recipientUserId);

            await hubContext.Clients
                .Group($"user_{recipientUserId}")
                .SendAsync("ReceiveNotification", new
                {
                    id = notification.Id,
                    actorName = notification.ActorName,
                    actorAvatarUrl = notification.ActorAvatarUrl,
                    title = notification.Title,
                    message = notification.Message,
                    type = notification.Type.ToString(),
                    actionUrl = notification.ActionUrl,
                    dateCreated = notification.DateCreated
                });

            await hubContext.Clients
                .Group($"user_{recipientUserId}")
                .SendAsync("UpdateUnreadCount", unreadCount);
        }

        public async Task MarkAsReadAsync(Guid notificationId, Guid userId)
        {
            var notification = await notificationRepository.GetByIdAsync(notificationId, userId);

            if (notification is null || notification.Status == NotificationStatus.Read)
            {
                return;
            }

            notification.Status = NotificationStatus.Read;
            notification.DateRead = DateTime.UtcNow;
            notification.DateModified = DateTime.UtcNow;

            notificationRepository.Update(notification);
            await unitOfWork.SaveAsync();

            var unreadCount = await notificationRepository.GetUnreadCountAsync(userId);

            await hubContext.Clients
                .Group($"user_{userId}")
                .SendAsync("UpdateUnreadCount", unreadCount);
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            var notifications = await notificationRepository.GetAllUnreadAsync(userId);

            if (notifications.Count == 0)
            {
                return;
            }

            foreach (var notification in notifications)
            {
                notification.Status = NotificationStatus.Read;
                notification.DateRead = DateTime.UtcNow;
                notification.DateModified = DateTime.UtcNow;
                notificationRepository.Update(notification);
            }

            await unitOfWork.SaveAsync();

            await hubContext.Clients
                .Group($"user_{userId}")
                .SendAsync("UpdateUnreadCount", 0);
        }

        public async Task MarkAsReadBySourceAsync(Guid userId, NotificationSourceEntityType sourceEntityType, Guid sourceEntityId)
        {
            var notifications = await notificationRepository.GetUnreadBySourceAsync(userId, sourceEntityType, sourceEntityId);

            if (notifications.Count == 0)
            {
                return;
            }

            foreach (var notification in notifications)
            {
                notification.Status = NotificationStatus.Read;
                notification.DateRead = DateTime.UtcNow;
                notification.DateModified = DateTime.UtcNow;
                notificationRepository.Update(notification);
            }

            await unitOfWork.SaveAsync();

            var unreadCount = await notificationRepository.GetUnreadCountAsync(userId);

            await hubContext.Clients
                .Group($"user_{userId}")
                .SendAsync("UpdateUnreadCount", unreadCount);
        }
    }
}