using Domain.Enums;

namespace Domain.Entities
{
    public class JobApplication
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid JobId { get; set; }

        public Job Job { get; set; } = default!;

        public Guid ProfessionalProfileId { get; set; }

        public ProfessionalProfile ProfessionalProfile { get; set; } = default!;

        public string CoverLetter { get; set; } = default!;

        public string ResumeUrl { get; set; } = default!;   

        public JobStatus JobStatus { get; set; }

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; }

        public string CreatedBy { get; set; } = default!;
    }
}
