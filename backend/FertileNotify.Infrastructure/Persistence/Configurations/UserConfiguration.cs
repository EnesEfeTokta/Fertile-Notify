using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FertileNotify.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email)
                .HasConversion(
                    email => email.Value,
                    value => new EmailAddress(value)
                )
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(u => u.PhoneNumber)
                .HasConversion(
                    phone => phone != null ? phone.Value : null,
                    value => value != null ? new PhoneNumber(value) : null
                )
                .HasMaxLength(20);

            builder.Property(u => u.ActiveChannels)
                .HasConversion(
                    v => string.Join(',', v.Select(c => c.Name)),
                    v => v.Split('s', StringSplitOptions.RemoveEmptyEntries)
                        .Select(NotificationChannel.From)
                        .ToHashSet())
                .Metadata.SetValueComparer(
                    new ValueComparer<IReadOnlyCollection<NotificationChannel>>(
                        (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToHashSet()));
        }
    }
}
