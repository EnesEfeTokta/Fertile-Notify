using FertileNotify.Domain.Enums;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Application.UseCases.RegisterUser
{
    public class RegisterUserCommand
    {
        public EmailAddress Email { get; init; } = default!;
        public PhoneNumber? PhoneNumber { get; init; } = default!;
        public SubscriptionPlan Plan { get; init; }
    }
}
