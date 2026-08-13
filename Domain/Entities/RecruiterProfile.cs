namespace Domain.Entities
{
    public class RecruiterProfile
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        public User User { get; set; } = default!;

        public Guid? CompanyId { get; set; }

        public Company? Company { get; set; } = default!;

        public string? JobTitle { get; set; } = default!;

        public bool IsCompanyAdmin { get; set; }

        public string? Department { get; set; } = default!;

        public RecruiterStatus Status { get; set; } = RecruiterStatus.Active;

        public bool IsDeleted { get; set; }

        public DateTime DateModified { get; set; }

        public string CreatedBy { get; set; } = default!;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public ICollection<Job> Jobs { get; set; } = new HashSet<Job>();
    }


    public enum RecruiterStatus
    {
        Pending,
        Active,
        Suspended
    }
}
