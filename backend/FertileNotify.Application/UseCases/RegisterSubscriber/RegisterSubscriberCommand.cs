namespace FertileNotify.Application.UseCases.RegisterSubscriber
{
    public class RegisterSubscriberCommand : ICommand<Guid>
    {
        public CompanyName CompanyName { get; init; } = default!;
        public Password Password { get; init; } = default!;
        public EmailAddress Email { get; init; } = default!;
        public PhoneNumber? PhoneNumber { get; init; } = default!;
        public SubscriptionPlan Plan { get; init; }
    }
}
