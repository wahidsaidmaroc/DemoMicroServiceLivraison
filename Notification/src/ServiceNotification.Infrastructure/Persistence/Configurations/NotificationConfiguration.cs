using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceNotification.Domain.Enums;

namespace ServiceNotification.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Domain.Entities.Notification>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id).ValueGeneratedNever();

        builder.Property(n => n.Channel)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(n => n.Recipient)
            .IsRequired()
            .HasMaxLength(320); // Max email length per RFC

        builder.Property(n => n.Subject)
            .HasMaxLength(500);

        builder.Property(n => n.Body)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(n => n.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(n => n.ProviderName)
            .HasMaxLength(100);

        builder.Property(n => n.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(n => n.CreatedAt)
            .IsRequired();

        builder.HasIndex(n => n.Status);
        builder.HasIndex(n => n.CreatedAt);
    }
}
