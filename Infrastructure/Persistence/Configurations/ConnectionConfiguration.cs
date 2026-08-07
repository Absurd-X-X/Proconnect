using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ConnectionConfiguration : IEntityTypeConfiguration<UserConnection>
{
    public void Configure(EntityTypeBuilder<UserConnection> builder)
    {
        builder.ToTable("UserConnection");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ConnectionStatus)
            .HasConversion<int>();


        builder.Property(x => x.DateCreated)
    .IsRequired();

        builder.HasIndex(x => new
        {
            x.SenderId,
            x.RecieverId
        }).IsUnique();

        builder.HasOne(x => x.Sender)
            .WithMany(x => x.SentConnections)
            .HasForeignKey(x => x.SenderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Reciever)
            .WithMany(x => x.ReceivedConnections)
            .HasForeignKey(x => x.RecieverId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}