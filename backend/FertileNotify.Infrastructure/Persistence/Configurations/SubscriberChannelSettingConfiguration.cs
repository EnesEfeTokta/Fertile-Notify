using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace FertileNotify.Infrastructure.Persistence.Configurations
{
    public class SubscriberChannelSettingConfiguration : IEntityTypeConfiguration<SubscriberChannelSetting>
    {
        public void Configure(EntityTypeBuilder<SubscriberChannelSetting> builder)
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

            builder.Property(e => e.Settings)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null)
                         ?? new Dictionary<string, string>()
                )
                .HasColumnType("jsonb");
        }
    }
}