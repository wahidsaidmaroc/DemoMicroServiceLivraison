using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceNotification.Domain.Entities;

namespace ServiceNotification.Infrastructure.Persistence.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedNever();

        builder.Property(o => o.Type)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.Payload)
            .IsRequired()
            .HasMaxLength(8000);

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.Error)
            .HasMaxLength(2000);

        builder.HasIndex(o => o.ProcessedAt);
    }
}
