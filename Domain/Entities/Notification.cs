using Domain.Enums;

namespace Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        public User User { get; set; } = default!;

        public string Title { get; set; } = default!;

        public string Message { get; set; } = default!;

        public NotificationType Type { get; set; }

        public bool IsRead { get; set; } = false;

        public NotificationStatus Status { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; } = default!;


    }
}
