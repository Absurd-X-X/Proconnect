namespace Domain.Entities
{
    public class Certificate
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProfessionalProfileId { get; set; }

        public ProfessionalProfile ProfessionalProfile { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string IssuingOrganization { get; set; } = default!;

        public DateTime IssueDate { get; set; }

        public DateTime? ExpireDate { get; set; }

        public string CredentialId { get; set; } = default!;

        public string CredentialUrl { get; set; } = default!;

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; } = default!;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
