using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FertileNotify.Infrastructure.Persistence.Configurations
{
    public class PaymentLogConfiguration : IEntityTypeConfiguration<PaymentLog>
    {
        public void Configure(EntityTypeBuilder<PaymentLog> builder)
        {
            builder.HasKey(l => l.Id);

            builder.HasIndex(l => l.StripePaymentIntentId)
                .IsUnique();

            builder.Property(l => l.SubscriberId)
                .IsRequired();

            builder.Property(l => l.StripePaymentIntentId)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(l => l.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(l => l.Status)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(l => l.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();
        }
    }
}
