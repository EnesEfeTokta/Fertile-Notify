using FertileNotify.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FertileNotify.Infrastructure.Persistence.Configurations
{
    public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
    {
        public void Configure(EntityTypeBuilder<ApiKey> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.KeyHash)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(a => a.Prefix)
                .IsRequired()
                .HasMaxLength(9);

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
