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

        public string? ResumeUrl { get; set; }

        public string? ResumePublicId { get; set; }

        public JobStatus JobStatus { get; set; }

        public DateTime? InterviewScheduledAt { get; set; }

        public string? InterviewType { get; set; }

        public string? InterviewLocationOrLink { get; set; }

        public string? ApplicantPhone { get; set; }

        public string? ApplicantLocation { get; set; }

        public string? ApplicantCurrentJobTitle { get; set; }

        public int? ApplicantYearsOfExperience { get; set; }

        public string? ApplicantLinkedInUrl { get; set; }

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; }

        public string CreatedBy { get; set; } = default!;

        public WorkAuthorizationStatus WorkAuthorization { get; set; }

        public string? AdditionalAnswers { get; set; }

        public Guid? LastActionedByRecruiterProfileId { get; set; }

        public RecruiterProfile? LastActionedByRecruiterProfile { get; set; }
    }
}