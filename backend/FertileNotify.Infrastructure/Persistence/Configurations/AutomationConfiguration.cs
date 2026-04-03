using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FertileNotify.Infrastructure.Persistence.Configurations
{
    public class AutomationConfiguration : IEntityTypeConfiguration<AutomationWorkflow>
    {
        public void Configure(EntityTypeBuilder<AutomationWorkflow> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(t => t.Channel)
                .HasConversion(
                    channel => channel.Name,
                    value => NotificationChannel.From(value)
                )
                .HasMaxLength(50)
                .IsRequired();

            var content = builder.OwnsOne(typeof(NotificationContent), nameof(AutomationWorkflow.Content));

            content.Property<string>(nameof(NotificationContent.Subject))
                .HasMaxLength(200)
                .IsRequired();

            content.Property<string>(nameof(NotificationContent.Body))
                .IsRequired();
            
            builder.Property(p => p.Recipients)
                .HasConversion(
                    recipients => string.Join(';', recipients),
                    value => value.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
                )
                .HasMaxLength(1500)
                .IsRequired();
        }
    }
}
