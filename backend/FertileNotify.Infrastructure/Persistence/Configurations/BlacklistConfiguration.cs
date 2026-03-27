using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FertileNotify.Infrastructure.Persistence.Configurations
{
    public class BlacklistConfiguration : IEntityTypeConfiguration<ForbiddenRecipient>
    {
        public void Configure(EntityTypeBuilder<ForbiddenRecipient> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.UnwantedSubscriber)
                .IsRequired();

            builder.Property(b => b.RecipientAddress)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(b => b.UnwantedChannels)
                .HasConversion(
                    channels => string.Join(',', channels.Select(c => c.Name)),
                    value => value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(NotificationChannel.From).ToList()
                )
                .Metadata.SetValueComparer(new ValueComparer<List<NotificationChannel>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                ));
            
            builder.Property(b => b.UnwantedChannels)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(b => b.IsActive)
                .IsRequired();

            builder.Property(b => b.CreatedAt)
                .IsRequired();
        }
    }
}
