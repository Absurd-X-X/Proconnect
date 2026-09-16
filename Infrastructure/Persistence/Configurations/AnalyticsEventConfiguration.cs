// AnalyticsEventConfiguration.cs — updated HasOne block, everything else unchanged
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class AnalyticsEventConfiguration : IEntityTypeConfiguration<AnalyticsEvent>
{
    public void Configure(EntityTypeBuilder<AnalyticsEvent> builder)
    {
        builder.ToTable("AnalyticsEvent");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EventType)
            .IsRequired();

        builder.Property(x => x.SubjectType)
            .IsRequired();

        builder.Property(x => x.SubjectId)
            .IsRequired();

        builder.Property(x => x.DateCreated)
            .IsRequired();

        builder.HasOne(x => x.ActorUser)
            .WithMany(x => x.AnalyticsEvents)
            .HasForeignKey(x => x.ActorUserId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasIndex(x => new { x.SubjectType, x.SubjectId, x.DateCreated });

        builder.HasIndex(x => new { x.ActorUserId, x.DateCreated });
    }
}