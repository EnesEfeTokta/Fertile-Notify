using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FertileNotify.Infrastructure.Persistence.Configurations
{
    public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
    {
        public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(t => t.Description)
                .HasMaxLength(250)
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

            builder.Property(t => t.SubjectTemplate)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(t => t.BodyTemplate)
                .IsRequired();
        }
    }
}