namespace FertileNotify.Domain.Entities
{
    public class WorkflowRecipientGroup
    {
        public string Channel { get; set; } = string.Empty;
        public List<string> Recipients { get; set; } = new();

        public WorkflowRecipientGroup() { }

        public WorkflowRecipientGroup(string channel, IEnumerable<string> recipients)
        {
            Channel = channel?.Trim() ?? string.Empty;
            Recipients = recipients
                .Where(recipient => !string.IsNullOrWhiteSpace(recipient))
                .Select(recipient => recipient.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}
