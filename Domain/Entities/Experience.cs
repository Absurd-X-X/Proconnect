using Domain.Enums;

namespace Domain.Entities
{
    public class Experience
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProfessionalProfileId { get; set; }

        public ProfessionalProfile ProfessionalProfile { get; set; } = default!;

        public string CompanyName { get; set; } = default!;

        public string JobTitle { get; set; } = default!;

        public EmploymentType EmploymentType { get; set; }

        public string Location { get; set; } = default!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsCurrentJob { get; set; }

        public string Description { get; set; } = default!;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; } = default!;
    }
}
