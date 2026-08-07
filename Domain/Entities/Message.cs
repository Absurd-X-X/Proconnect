namespace Domain.Entities
{
    public class Message
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ConversationId { get; set; }

        public Conversation Conversation { get; set; } = default!;

        public Guid UserId { get; set; }

        public User User { get; set; } = default!;

        public string Content { get; set; } = default!;

        public bool IsRead { get; set; } = false;

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; } = default!;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
