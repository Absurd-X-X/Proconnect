using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        builder.ToTable("JobApplication");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ResumeUrl)
            .HasMaxLength(500);

        builder.Property(x => x.JobStatus)
            .HasConversion<int>();

        builder.Property(x => x.AppliedAt)
    .IsRequired();

        builder.HasIndex(x => new
        {
            x.JobId,
            x.ProfessionalProfileId
        }).IsUnique();

        builder.HasOne(x => x.Job)
            .WithMany(x => x.JobApplications)
            .HasForeignKey(x => x.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ProfessionalProfile)
            .WithMany(x => x.JobApplications)
            .HasForeignKey(x => x.ProfessionalProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}