using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ConversationParticipantConfiguration : IEntityTypeConfiguration<ConversationParticipant>
{
    public void Configure(EntityTypeBuilder<ConversationParticipant> builder)
    {
        builder.ToTable("ConversationParticipant");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.JoinedAt)
    .IsRequired();

        builder.HasIndex(x => new
        {
            x.ConversationId,
            x.UserId
        }).IsUnique();



        builder.HasOne(x => x.Conversation)
            .WithMany(x => x.Participants)
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Participants)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}