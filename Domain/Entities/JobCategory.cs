namespace Domain.Entities
{
    public class JobCategory
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = default!;

        public string Description { get; set; } = default!;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; } = default!;

        public ICollection<Job> Jobs { get; set; } = new HashSet<Job>(); 
    }
}
