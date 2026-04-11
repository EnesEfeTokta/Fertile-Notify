using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FertileNotify.Infrastructure.Persistence.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            var content = builder.OwnsOne(typeof(NotificationContent), nameof(Notification.Content));

            content.Property<string>(nameof(NotificationContent.Subject))
                .HasColumnName("Subject")
                .HasMaxLength(200)
                .IsRequired();

            content.Property<string>(nameof(NotificationContent.Body))
                .HasColumnName("Body")
                .IsRequired();

            builder.Property(n => n.CreateAt)
                .IsRequired();
        }
    }
}
