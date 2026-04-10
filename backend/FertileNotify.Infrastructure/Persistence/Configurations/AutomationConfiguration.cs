using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FertileNotify.Infrastructure.Persistence.Configurations
{
    public class AutomationConfiguration : IEntityTypeConfiguration<AutomationWorkflow>
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        public void Configure(EntityTypeBuilder<AutomationWorkflow> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(t => t.EventType)
                .HasMaxLength(50)
                .IsRequired();

            var content = builder.OwnsOne(typeof(NotificationContent), nameof(AutomationWorkflow.Content));

            content.Property<string>(nameof(NotificationContent.Subject))
                .HasMaxLength(200)
                .IsRequired();

            content.Property<string>(nameof(NotificationContent.Body))
                .IsRequired();
            
            var recipientGroupsProperty = builder.Property(p => p.To)
                .HasConversion(
                    groups => JsonSerializer.Serialize(groups, JsonOptions),
                    value => string.IsNullOrWhiteSpace(value)
                        ? new List<WorkflowRecipientGroup>()
                        : JsonSerializer.Deserialize<List<WorkflowRecipientGroup>>(value, JsonOptions) ?? new List<WorkflowRecipientGroup>()
                )
                .HasColumnName("RecipientGroups")
                .HasColumnType("text")
                .IsRequired();

            recipientGroupsProperty.Metadata.SetValueComparer(new ValueComparer<List<WorkflowRecipientGroup>>(
                    (left, right) => SerializeRecipientGroups(left) == SerializeRecipientGroups(right),
                    groups => StringComparer.Ordinal.GetHashCode(SerializeRecipientGroups(groups)),
                    groups => DeserializeRecipientGroups(SerializeRecipientGroups(groups))));
        }

        private static string SerializeRecipientGroups(List<WorkflowRecipientGroup>? groups)
            => JsonSerializer.Serialize(groups ?? new List<WorkflowRecipientGroup>(), JsonOptions);

        private static List<WorkflowRecipientGroup> DeserializeRecipientGroups(string json)
            => JsonSerializer.Deserialize<List<WorkflowRecipientGroup>>(json, JsonOptions) ?? new List<WorkflowRecipientGroup>();
    }
}
