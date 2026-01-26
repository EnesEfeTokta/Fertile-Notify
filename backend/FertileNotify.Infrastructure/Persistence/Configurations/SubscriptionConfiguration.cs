using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FertileNotify.Infrastructure.Persistence.Configurations
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.HasKey(s => s.Id);
            
            builder.Property(s => s.AllowedEvents)
                .HasConversion(
                    v => string.Join(',', v.Select(e => e.Name)),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(EventType.From)
                          .ToHashSet())
                .Metadata.SetValueComparer(
                    new ValueComparer<IReadOnlyCollection<EventType>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                     c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                     c => c.ToHashSet()));
        }
    }
}