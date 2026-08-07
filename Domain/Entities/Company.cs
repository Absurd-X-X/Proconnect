namespace Domain.Entities
{
    public class Company
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = default!;

        public string Industry { get; set; } = default!;

        public string Description { get; set; } = default!;

        public string? Website { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string PhoneNumber { get; set; } = default!;

        public string? Logo { get; set; } = default!;

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; } = default!;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public ICollection<Job> Jobs { get; set; } = new HashSet<Job>();

        public ICollection<RecruiterProfile> RecruiterProfiles { get; set; } = new HashSet<RecruiterProfile>();
    }
}
