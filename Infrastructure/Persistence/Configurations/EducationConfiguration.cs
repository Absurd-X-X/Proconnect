using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class EducationConfiguration : IEntityTypeConfiguration<Education>
{
    public void Configure(EntityTypeBuilder<Education> builder)
    {
        builder.ToTable("Education");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Institution)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Degree)
            .HasMaxLength(150);

        builder.Property(x => x.FieldOfStudy)
            .HasMaxLength(150);

        builder.Property(x => x.Grade)
            .HasMaxLength(50);

        builder.Property(x => x.DateCreated)
    .IsRequired();

        builder.HasOne(x => x.ProfessionalProfile)
            .WithMany(x => x.Educations)
            .HasForeignKey(x => x.ProfessionalProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}