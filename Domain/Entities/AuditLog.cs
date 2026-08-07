namespace Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        public User User { get; set; } = default!;

        public string Action { get; set; } = default!;

        public string Description { get; set; } = default!;

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; } = default!;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
