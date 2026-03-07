using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FertileNotify.Infrastructure.Persistence.Configurations
{
    public class StatsConfiguration : IEntityTypeConfiguration<SubscriberDailyStats>
    {
        public void Configure(EntityTypeBuilder<SubscriberDailyStats> builder)
        {
            builder.HasKey(s => s.Id);

            builder.HasIndex(s => new { s.SubscriberId, s.Channel });

            builder.Property(s => s.Channel)
                .HasConversion(
                    channel => channel.Name,
                    value => NotificationChannel.From(value)
                )
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(t => t.EventType)
                .HasConversion(
                    eventType => eventType.Name,
                    value => EventType.From(value)
                )
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}
