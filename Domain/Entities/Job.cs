using Domain.Enums;

namespace Domain.Entities
{
    public class Job
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CompanyId { get; set; }

        public Company Company { get; set; } = default!;

        public Guid RecruiterProfileId { get; set; }

        public RecruiterProfile RecruiterProfile { get; set; } = default!;

        public Guid JobCategoryId { get; set; }

        public JobCategory Category { get; set; } = default!;

        public string Title { get; set; } = default!;

        public string Description { get; set; } = default!;

        public string Requirement { get; set; } = default!;

        public EmploymentType EmploymentType { get; set; }

        public WorkPlaceType WorkPlaceType { get; set; }

        public ExperienceLevel ExperienceLevel { get; set; }

        public decimal MinSalary { get; set; }

        public decimal MaxSalary { get; set; }

        public string Currency { get; set; } = default!;

        public string Location { get; set; } = default!;

        public DateTime ApplicationDeadline { get; set; }

        public bool IsActive { get; set; } = true;

        public string CreatedBy { get; set; } = default!;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; }

        public ICollection<SavedJob> SavedJobs { get; set; } = new HashSet<SavedJob>();

        public ICollection<JobApplication> JobApplications { get; set; } = new HashSet<JobApplication>();
    }
}
