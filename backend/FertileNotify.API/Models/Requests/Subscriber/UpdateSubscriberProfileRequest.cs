namespace FertileNotify.API.Models.Requests
{
    public class UpdateSubscriberProfileRequest
    {
        public string? CompanyName { get; init; }
        public string? CompanyDescription { get; init; }
        public string? LogoUrl { get; init; }
        public string? WebsiteUrl { get; init; }
        public string? Location { get; init; }

        public string? Email { get; init; }
        public string? PhoneNumber { get; init; }
    }
}
