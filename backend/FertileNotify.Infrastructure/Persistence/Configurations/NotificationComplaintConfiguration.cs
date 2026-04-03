using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FertileNotify.Infrastructure.Persistence.Configurations
{
    public class NotificationComplaintConfiguration : IEntityTypeConfiguration<NotificationComplaint>
    {
        public void Configure(EntityTypeBuilder<NotificationComplaint> builder) 
        {
            builder.HasKey(nc => nc.Id);

            builder.Property(nc => nc.ReporterEmail)
                .HasConversion(
                    email => email.Value,
                    value => new Domain.ValueObjects.EmailAddress(value)
                )
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(nc => nc.Reason)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(nc => nc.Description)
                .HasMaxLength(500);

            var content = builder.OwnsOne(typeof(NotificationContent), nameof(NotificationComplaint.Content));

            content.Property<string>(nameof(NotificationContent.Subject))
                .HasColumnName("NotificationSubject")
                .HasMaxLength(200)
                .IsRequired();

            content.Property<string>(nameof(NotificationContent.Body))
                .HasColumnName("NotificationBody")
                .IsRequired();
        }
    }
}
