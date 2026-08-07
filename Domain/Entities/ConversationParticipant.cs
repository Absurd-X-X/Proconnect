namespace Domain.Entities
{
    public class ConversationParticipant
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ConversationId { get; set; }

        public Conversation Conversation { get; set; } = null!;

        public Guid UserId { get; set; }

        public User User { get; set; } = default!;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; } = default!;
    }
}
