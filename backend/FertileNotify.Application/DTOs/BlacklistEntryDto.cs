namespace FertileNotify.Application.DTOs
{
    public class BlacklistEntryDto
    {
        public Guid Id { get; set; }
        public string RecipientAddress { get; set; } = string.Empty;
        public List<string> Channels { get; set; } = new();
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
