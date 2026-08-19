using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProfessionalProfileConfiguration : IEntityTypeConfiguration<ProfessionalProfile>
{
    public void Configure(EntityTypeBuilder<ProfessionalProfile> builder)
    {
        builder.ToTable("ProfessionalProfiles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.HeadLine)
            .HasMaxLength(255);

        builder.Property(x => x.GitHubUrl)
            .HasMaxLength(500);

        builder.Property(x => x.LinkedInUrl)
            .HasMaxLength(500);

        builder.Property(x => x.ResumeUrl)
            .HasMaxLength(500);

        builder.Property(x => x.ResumePublicId)
            .HasMaxLength(255);

        builder.Property(x => x.UserStatus)
            .HasConversion<string>()
            .HasMaxLength(100);

        builder.Property(x => x.AvailabilityStatus)
    .HasConversion<string>()
    .HasMaxLength(50);

        builder.Property(x => x.WorkAuthorization)
            .HasConversion<string>()
            .HasMaxLength(100);

        builder.Property(x => x.AvailabilityVisibility)
            .HasConversion<string>()
            .HasMaxLength(100);

        builder.Property(x => x.WebsiteUrl)
    .HasMaxLength(500);

        builder.Property(x => x.PreferredJobTypes)
            .HasConversion(
                v => string.Join(',', v),
                v => v == ""
                    ? new List<EmploymentType>()
                    : v.Split(',', StringSplitOptions.None).Select(x => Enum.Parse<EmploymentType>(x)).ToList())
            .Metadata.SetValueComparer(new ValueComparer<List<EmploymentType>>(
                (a, b) => a!.SequenceEqual(b!),
                v => v.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
                v => v.ToList()));

        builder.Property(x => x.PreferredLocations)
            .HasConversion(
                v => string.Join('|', v),
                v => v == ""
                    ? new List<string>()
                    : v.Split('|', StringSplitOptions.None).ToList())
            .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (a, b) => a!.SequenceEqual(b!),
                v => v.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
                v => v.ToList()));

        builder.Property(x => x.DateCreated)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithOne(x => x.ProfessionalProfile)
            .HasForeignKey<ProfessionalProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.PortfolioLinks)
            .WithOne(x => x.ProfessionalProfile)
            .HasForeignKey(x => x.ProfessionalProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}