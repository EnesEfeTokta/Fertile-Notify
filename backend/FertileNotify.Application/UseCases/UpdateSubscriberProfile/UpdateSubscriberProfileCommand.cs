namespace FertileNotify.Application.UseCases.UpdateSubscriberProfile
{
    public class UpdateSubscriberProfileCommand : IRequest<Unit>
    {
        public Guid SubscriberId { get; init; }

        public string? CompanyName { get; init; }
        public string? CompanyDescription { get; init; }
        public string? LogoUrl { get; init; }
        public string? WebsiteUrl { get; init; }
        public string? Location { get; init; }

        public string? Email { get; init; }
        public string? PhoneNumber { get; init; }
    }
}