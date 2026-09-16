namespace Domain.Entities
{
    public class EventAgendaItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid EventId { get; set; }

        public Event Event { get; set; } = default!;

        public string Title { get; set; } = default!;

        public string? Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int DisplayOrder { get; set; }
    }
}