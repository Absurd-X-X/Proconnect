using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class FileUploadConfiguration : IEntityTypeConfiguration<FileUpload>
{
    public void Configure(EntityTypeBuilder<FileUpload> builder)
    {
        builder.ToTable("FileUpload");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileName)
            .HasMaxLength(255);

        builder.Property(x => x.FileUrl)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.FileType)
            .HasMaxLength(100);

        builder.Property(x => x.UploadedOn)
    .IsRequired();

        builder.HasOne(v => v.Post)
            .WithMany(v => v.Attachments)
            .HasForeignKey(v => v.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Files)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}