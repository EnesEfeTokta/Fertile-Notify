namespace FertileNotify.Application.UseCases.RegisterSubscriber
{
    public class RegisterSubscriberCommand : ICommand<Guid>
    {
        public CompanyName CompanyName { get; init; } = default!;
        public string CompanyDescription { get; init; } = string.Empty;
        public CustomUrl? LogoUrl { get; init; } = default!;
        public CustomUrl? WebsiteUrl { get; init; } = default!;
        public string Location { get; init; } = string.Empty;
        public Password Password { get; init; } = default!;
        public EmailAddress Email { get; init; } = default!;
        public PhoneNumber? PhoneNumber { get; init; } = default!;
        public SubscriptionPlan Plan { get; init; }
    }
}
