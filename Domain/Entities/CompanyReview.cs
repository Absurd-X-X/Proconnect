namespace Domain.Entities
{
    public class CompanyReview
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CompanyId { get; set; }
        public Company Company { get; set; } = default!;

        public Guid ReviewerId { get; set; }
        public User Reviewer { get; set; } = default!;

        public int OverallRating { get; set; }
        public int WorkLifeBalanceRating { get; set; }
        public int CompensationRating { get; set; }
        public int JobSecurityRating { get; set; }
        public int ManagementRating { get; set; }
        public int CultureRating { get; set; }

        public string Title { get; set; } = default!;
        public string Content { get; set; } = default!;
        public string JobTitle { get; set; } = default!;
        public string? Location { get; set; }
        public bool IsCurrentEmployee { get; set; }
        public int? YearsAtCompany { get; set; }

        public int HelpfulCount { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; } = default!;
    }
}