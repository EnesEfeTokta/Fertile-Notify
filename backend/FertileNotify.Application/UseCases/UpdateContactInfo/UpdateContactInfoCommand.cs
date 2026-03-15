namespace FertileNotify.Application.UseCases.UpdateContactInfo
{
    public class UpdateContactInfoCommand
    {
        public Guid SubscriberId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }
}
