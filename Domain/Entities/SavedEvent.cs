namespace Domain.Entities
{
    public class SavedEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid EventId { get; set; }

        public Event Event { get; set; } = default!;

        public Guid ProfessionalProfileId { get; set; }

        public ProfessionalProfile ProfessionalProfile { get; set; } = default!;

        public string CreatedBy { get; set; } = default!;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}