using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Post");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content)
            .IsRequired();


        builder.Property(x => x.Visibility)
            .HasConversion<int>();

        builder.Property(x => x.DateCreated)
    .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.Posts)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}