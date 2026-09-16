using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class JobSkillConfiguration : IEntityTypeConfiguration<JobSkill>
{
    public void Configure(EntityTypeBuilder<JobSkill> builder)
    {
        builder.ToTable("JobSkill");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DateCreated)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.JobId,
            x.SkillId
        }).IsUnique();

        builder.HasOne(x => x.Job)
            .WithMany(x => x.JobSkills)
            .HasForeignKey(x => x.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Skill)
            .WithMany(x => x.JobSkills)
            .HasForeignKey(x => x.SkillId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}