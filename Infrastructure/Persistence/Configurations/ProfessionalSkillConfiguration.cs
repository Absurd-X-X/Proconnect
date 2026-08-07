using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProfessionalSkillConfiguration : IEntityTypeConfiguration<ProfessionalSkill>
{
    public void Configure(EntityTypeBuilder<ProfessionalSkill> builder)
    {
        builder.ToTable("ProfessionalSkill");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Level)
            .HasMaxLength(50);

        builder.Property(x => x.DateCreated)
    .IsRequired();

        builder.HasIndex(x => new
        {
            x.ProfessionalProfileId,
            x.SkillId
        }).IsUnique();

        builder.HasOne(x => x.ProfessionalProfile)
            .WithMany(x => x.ProfessionalSkills)
            .HasForeignKey(x => x.ProfessionalProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Skill)
            .WithMany(x => x.ProfessionalSkills)
            .HasForeignKey(x => x.SkillId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}