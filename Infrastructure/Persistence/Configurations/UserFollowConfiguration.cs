using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserFollowConfiguration : IEntityTypeConfiguration<UserFollow>
{
    public void Configure(EntityTypeBuilder<UserFollow> builder)
    {
        builder.ToTable("UserFollow");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DateCreated)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.FollowerId,
            x.FollowingId
        }).IsUnique();

        builder.HasOne(x => x.Follower)
            .WithMany(x => x.Following)
            .HasForeignKey(x => x.FollowerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Following)
            .WithMany(x => x.Followers)
            .HasForeignKey(x => x.FollowingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}