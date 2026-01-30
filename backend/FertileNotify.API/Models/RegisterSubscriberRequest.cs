namespace FertileNotify.API.Models
{
    public class RegisterSubscriberRequest
    {
        public string CompanyName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string? PhoneNumber { get; init; } = string.Empty;
        public string Plan { get; init; } = string.Empty;
    }
}
