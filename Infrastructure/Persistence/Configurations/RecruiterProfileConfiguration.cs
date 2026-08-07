using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class RecruiterProfileConfiguration : IEntityTypeConfiguration<RecruiterProfile>
{
    public void Configure(EntityTypeBuilder<RecruiterProfile> builder)
    {
        builder.ToTable("RecruiterProfiles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.JobTitle)
            .HasMaxLength(150);

        builder.Property(x => x.Department)
            .HasMaxLength(150);

        builder.Property(x => x.IsCompanyAdmin)
            .HasDefaultValue(false);

        builder.Property(x => x.DateCreated)
    .IsRequired();

        builder.HasOne(x => x.User)
            .WithOne(x => x.RecruiterProfile)
            .HasForeignKey<RecruiterProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Company)
            .WithMany(x => x.RecruiterProfiles)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}