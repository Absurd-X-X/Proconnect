using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SavedJobConfiguration : IEntityTypeConfiguration<SavedJob>
{
    public void Configure(EntityTypeBuilder<SavedJob> builder)
    {
        builder.ToTable("SavedJob");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SavedAt)
    .IsRequired();

        builder.HasIndex(x => new
        {
            x.ProfessionalProfileId,
            x.JobId
        }).IsUnique();

        builder.HasOne(x => x.ProfessionalProfile)
            .WithMany(x => x.SavedJobs)
            .HasForeignKey(x => x.ProfessionalProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Job)
            .WithMany(x => x.SavedJobs)
            .HasForeignKey(x => x.JobId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}