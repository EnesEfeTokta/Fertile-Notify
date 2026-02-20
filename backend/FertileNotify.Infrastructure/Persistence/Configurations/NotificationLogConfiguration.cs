using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FertileNotify.Infrastructure.Persistence.Configurations
{
    public class NotificationLogConfiguration : IEntityTypeConfiguration<NotificationLog>
    {
        public void Configure(EntityTypeBuilder<NotificationLog> builder)
        {
            builder.HasKey(l => l.Id);

            builder.Property(l => l.Recipient)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(t => t.EventType)
                .HasConversion(
                    eventType => eventType.Name,
                    value => EventType.From(value)
                )
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(t => t.Channel)
                .HasConversion(
                    channel => channel.Name,
                    value => NotificationChannel.From(value)
                )
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(l => l.Subject)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(l => l.Body)
                .IsRequired();
        }
    }
}
