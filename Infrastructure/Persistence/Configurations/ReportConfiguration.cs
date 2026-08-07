using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable("Report");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Reason)
            .IsRequired();

        builder.Property(x => x.ReportStatus)
            .HasConversion<int>();

        builder.Property(x => x.DateCreated)
    .IsRequired();

        builder.HasOne(x => x.Reporter)
            .WithMany(x => x.ReportsSubmitted)
            .HasForeignKey(x => x.ReporterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ReportedUser)
            .WithMany(x => x.ReportsReceived)
            .HasForeignKey(x => x.ReportedUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Post)
            .WithMany(x => x.Reports)
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}