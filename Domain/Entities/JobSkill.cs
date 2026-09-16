namespace Domain.Entities
{
    public class JobSkill
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid JobId { get; set; }

        public Job Job { get; set; } = default!;

        public Guid SkillId { get; set; }

        public Skill Skill { get; set; } = default!;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public string CreatedBy { get; set; } = default!;

        public bool IsDeleted { get; set; }
    }
}