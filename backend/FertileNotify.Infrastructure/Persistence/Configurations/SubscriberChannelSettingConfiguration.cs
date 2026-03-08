using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

            builder.Property(x => x.Settings)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, string>())
                .Metadata.SetValueComparer(new ValueComparer<IReadOnlyDictionary<string, string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.Key.GetHashCode(), v.Value.GetHashCode())),
                    c => c.ToDictionary(x => x.Key, x => x.Value)));
        }
    }
}