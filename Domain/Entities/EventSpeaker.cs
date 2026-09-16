namespace Domain.Entities
{
    public class EventSpeaker
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid EventId { get; set; }

        public Event Event { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string? Title { get; set; }

        public string? Company { get; set; }

        public string? PhotoUrl { get; set; }

        public string? LinkedInUrl { get; set; }

        public int DisplayOrder { get; set; }
    }
}