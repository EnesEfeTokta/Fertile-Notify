using FertileNotify.Domain.Enums;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Application.UseCases.RegisterSubscriber
{
    public class RegisterSubscriberCommand
    {
        public CompanyName CompanyName { get; init; } = default!;
        public EmailAddress Email { get; init; } = default!;
        public PhoneNumber? PhoneNumber { get; init; } = default!;
        public SubscriptionPlan Plan { get; init; }
    }
}
