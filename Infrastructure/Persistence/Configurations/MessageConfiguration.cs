using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Message");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DateCreated)
            .IsRequired();

        builder.HasOne(x => x.Conversation)
            .WithMany(x => x.Messages)
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(x => x.SentMessages)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}