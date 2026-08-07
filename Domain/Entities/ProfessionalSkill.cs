namespace Domain.Entities
{
    public class ProfessionalSkill
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProfessionalProfileId { get; set; }

        public ProfessionalProfile ProfessionalProfile { get; set; } = default!;

        public Guid SkillId {  get; set; }

        public Skill Skill { get; set; } = default!;

        public string Level { get; set; } = default!;

        public int YearsOfExperience { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public string CreatedBy { get; set; } = default!;
    }
}
