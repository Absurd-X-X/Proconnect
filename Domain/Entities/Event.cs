using Domain.Enums;

namespace Domain.Entities
{
    public class Event
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CompanyId { get; set; }

        public Company Company { get; set; } = default!;

        public Guid RecruiterProfileId { get; set; }

        public RecruiterProfile RecruiterProfile { get; set; } = default!;

        public string Title { get; set; } = default!;

        public string Description { get; set; } = default!;

        public string? CoverImageUrl { get; set; }

        public string? CoverImagePublicId { get; set; }

        public EventType EventType { get; set; }

        public string? Category { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public EventLocationType LocationType { get; set; }

        public string? Location { get; set; }

        public string? OnlineMeetingLink { get; set; }

        public EventVisibility Visibility { get; set; } = EventVisibility.Public;

        public EventRegistrationType RegistrationType { get; set; } = EventRegistrationType.Open;

        public int? AttendeeLimit { get; set; }

        public EventStatus Status { get; set; } = EventStatus.Draft;

        public DateTime? PublishedAt { get; set; }

        public DateTime? CancelledAt { get; set; }

        public string? CancellationReason { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; } = default!;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime? DateModified { get; set; }

        public ICollection<EventSpeaker> Speakers { get; set; } = new HashSet<EventSpeaker>();

        public ICollection<EventAgendaItem> AgendaItems { get; set; } = new HashSet<EventAgendaItem>();

        public ICollection<EventRegistration> Registrations { get; set; } = new HashSet<EventRegistration>();

        public ICollection<SavedEvent> SavedByProfessionals { get; set; } = new HashSet<SavedEvent>();
    }
}