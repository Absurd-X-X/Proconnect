using Domain.Entities;
using Microsoft.EntityFrameworkCore;
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

        builder.Property(x => x.PortfolioUrl)
            .HasMaxLength(500);

        builder.Property(x => x.GitHubUrl)
            .HasMaxLength(500);

        builder.Property(x => x.LinkedInUrl)
            .HasMaxLength(500);

        builder.Property(x => x.ResumeUrl)
            .HasMaxLength(500);

        builder.Property(x => x.UserStatus)
            .HasConversion<string>()
            .HasMaxLength(100);

        builder.Property(x => x.DateCreated)
    .IsRequired();

        builder.HasOne(x => x.User)
            .WithOne(x => x.ProfessionalProfile)
            .HasForeignKey<ProfessionalProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}