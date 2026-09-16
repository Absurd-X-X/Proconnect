using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notification");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Message)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.SourceEntityType)
            .HasConversion<int?>();

        builder.Property(x => x.ActionUrl)
            .HasMaxLength(500);

        builder.Property(x => x.ActorName)
            .HasMaxLength(200);

        builder.Property(x => x.ActorAvatarUrl)
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .HasDefaultValue(Domain.Enums.NotificationStatus.Unread)
            .IsRequired();

        builder.Property(x => x.DateCreated)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(450)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.Notifications)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ActorUser)
            .WithMany()
            .HasForeignKey(x => x.ActorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.UserId, x.IsDeleted, x.Status, x.DateCreated });
    }
}