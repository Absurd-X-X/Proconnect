using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.ToTable("Certificate");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.IssuingOrganization)
            .HasMaxLength(200);

        builder.Property(x => x.CredentialId)
            .HasMaxLength(150);

        builder.Property(x => x.CredentialUrl)
            .HasMaxLength(500);

        builder.Property(x => x.DateCreated)
            .IsRequired();

        builder.HasOne(x => x.ProfessionalProfile)
            .WithMany(x => x.Certificates)
            .HasForeignKey(x => x.ProfessionalProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}