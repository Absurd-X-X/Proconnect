using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Project");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.ProjectUrl)
            .HasMaxLength(500);

        builder.Property(x => x.ProjectUrl)
            .HasMaxLength(500);

        builder.Property(x => x.DateCreated)
    .IsRequired();

        builder.HasOne(x => x.ProfessionalProfile)
            .WithMany(x => x.Projects)
            .HasForeignKey(x => x.ProfessionalProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}