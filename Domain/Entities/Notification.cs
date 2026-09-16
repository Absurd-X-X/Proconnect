using Domain.Enums;

namespace Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        public Guid? ActorUserId { get; set; }
        public User? ActorUser { get; set; }

        public string? ActorName { get; set; }
        public string? ActorAvatarUrl { get; set; }

        public string Title { get; set; } = default!;

        public string Message { get; set; } = default!;

        public NotificationType Type { get; set; }

        public NotificationSourceEntityType? SourceEntityType { get; set; }
        public Guid? SourceEntityId { get; set; }
        public string? ActionUrl { get; set; }

        public NotificationStatus Status { get; set; } = NotificationStatus.Unread;
        public DateTime? DateRead { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; } = default!;
    }
}