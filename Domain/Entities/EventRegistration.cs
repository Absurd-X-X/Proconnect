using Domain.Enums;

namespace Domain.Entities
{
    public class EventRegistration
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid EventId { get; set; }

        public Event Event { get; set; } = default!;

        public Guid ProfessionalProfileId { get; set; }

        public ProfessionalProfile ProfessionalProfile { get; set; } = default!;

        public string FullName { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string Phone { get; set; } = default!;

        public string? CompanyOrganization { get; set; }

        public string? JobTitle { get; set; }

        public string? GoalsForAttending { get; set; }

        public string? ResumeUrl { get; set; }

        public string? ResumePublicId { get; set; }

        public EventRegistrationStatus Status { get; set; } = EventRegistrationStatus.Registered;

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        public DateTime? CancelledAt { get; set; }

        public string CreatedBy { get; set; } = default!;
    }
}