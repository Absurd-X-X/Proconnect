namespace Domain.Entities
{
    public class Education
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProfessionalProfileId { get; set; }

        public ProfessionalProfile ProfessionalProfile { get; set; } = default!;

        public string Institution { get; set; } = default!;

        public string Degree { get; set; } = default!;

        public string FieldOfStudy { get; set; } = default!;

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Grade { get; set; } = default!;

        public string? Description { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime DateModified { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; } = default!;
    }
}