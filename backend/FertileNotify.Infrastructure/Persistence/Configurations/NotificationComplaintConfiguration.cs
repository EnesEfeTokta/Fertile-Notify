using FertileNotify.Domain.Entities;
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

            builder.Property(nc => nc.NotificationSubject)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(nc => nc.NotificationBody)
                .IsRequired();
        }
    }
}
