namespace Domain.Entities
{
    public class Conversation
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public bool IsGroup { get; set; }

        public string? Title { get; set; }

        public string? GroupPhotoUrl { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; } = default!;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime DateModified { get; set; }

        public ICollection<Message> Messages { get; set; } = new HashSet<Message>();

        public ICollection<ConversationParticipant> Participants { get; set; } = new HashSet<ConversationParticipant>();
    }
}