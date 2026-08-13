using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PortfolioLinkConfiguration : IEntityTypeConfiguration<PortfolioLink>
{
    public void Configure(EntityTypeBuilder<PortfolioLink> builder)
    {
        builder.ToTable("PortfolioLinks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Url)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.LinkType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedBy)
            .IsRequired();

        builder.Property(x => x.DateCreated)
            .IsRequired();

        builder.Property(x => x.ThumbnailUrl)
    .HasMaxLength(500);

        builder.Property(x => x.ThumbnailPublicId)
            .HasMaxLength(255);
    }
}