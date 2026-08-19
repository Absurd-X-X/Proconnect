using Domain.Enums;

namespace Domain.Entities
{
    public class ProfessionalProfile
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        public User User { get; set; } = default!;

        public string? HeadLine { get; set; } = default!;

        public string? Summary { get; set; } = default!;

        public string? GitHubUrl { get; set; } = default!;

        public string? LinkedInUrl { get; set; } = default!;

        public string? WebsiteUrl { get; set; } = default!;

        public string? ResumeUrl { get; set; } = default!;

        public string? ResumePublicId { get; set; }

        public string? ResumeFileName { get; set; }

        public long? ResumeFileSizeBytes { get; set; }

        public DateTime? ResumeUploadedAt { get; set; }

        public int ResumeViewCount { get; set; }

        public int ResumeDownloadCount { get; set; }

        public UserStatus UserStatus { get; set; }

        public AvailabilityStatus AvailabilityStatus { get; set; }

        public List<EmploymentType> PreferredJobTypes { get; set; } = new();

        public List<string> PreferredLocations { get; set; } = new();

        public DateTime? EarliestStartDate { get; set; }

        public bool WillingToRelocate { get; set; }

        public WorkAuthorizationStatus WorkAuthorization { get; set; }

        public AvailabilityVisibility AvailabilityVisibility { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; } = default!;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime DateModified { get; set; }

        public ICollection<SavedJob> SavedJobs { get; set; } = new HashSet<SavedJob>();

        public ICollection<Experience> Experiences { get; set; } = new HashSet<Experience>();

        public ICollection<Certificate> Certificates { get; set; } = new HashSet<Certificate>();

        public ICollection<Project> Projects { get; set; } = new HashSet<Project>();

        public ICollection<Education> Educations { get; set; } = new HashSet<Education>();

        public ICollection<ProfessionalSkill> ProfessionalSkills { get; set; } = new HashSet<ProfessionalSkill>();

        public ICollection<JobApplication> JobApplications { get; set; } = new HashSet<JobApplication>();

        public ICollection<PortfolioLink> PortfolioLinks { get; set; } = new HashSet<PortfolioLink>();
    }
}