using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ExperienceConfiguration : IEntityTypeConfiguration<Experience>
{
    public void Configure(EntityTypeBuilder<Experience> builder)
    {
        builder.ToTable("Experience");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CompanyName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.JobTitle)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.EmploymentType)
            .HasConversion<string>()
            .HasMaxLength(100);

        builder.Property(x => x.Location)
            .HasMaxLength(150);

        builder.Property(x => x.IsCurrentJob)
            .HasDefaultValue(false);

        builder.Property(x => x.DateCreated)
    .IsRequired();

        builder.HasOne(x => x.ProfessionalProfile)
            .WithMany(x => x.Experiences)
            .HasForeignKey(x => x.ProfessionalProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}