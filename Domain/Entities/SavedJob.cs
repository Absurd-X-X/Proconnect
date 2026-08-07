namespace Domain.Entities
{
    public class SavedJob
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProfessionalProfileId { get; set; }

        public ProfessionalProfile ProfessionalProfile { get; set; } = default!;

        public Guid JobId { get; set; }

        public Job Job { get; set; } = default!;

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; } = default!;

        public DateTime SavedAt { get; set; } = DateTime.UtcNow;
    }
}
