using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PostLikeConfiguration : IEntityTypeConfiguration<PostLike>
{
    public void Configure(EntityTypeBuilder<PostLike> builder)
    {
        builder.ToTable("PostLike");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DateCreated)
    .IsRequired();

        builder.HasIndex(x => new
        {
            x.PostId,
            x.UserId
        }).IsUnique();

        builder.Property(x => x.ReactionType)
            .HasConversion<int>();

        builder.HasOne(x => x.Post)
            .WithMany(x => x.PostLikes)
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(x => x.PostLikes)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}