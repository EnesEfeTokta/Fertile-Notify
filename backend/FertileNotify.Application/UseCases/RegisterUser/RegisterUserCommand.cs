using FertileNotify.Domain.Enums;

namespace FertileNotify.Application.UseCases.RegisterUser
{
    public class RegisterUserCommand
    {
        public string Email { get; init; } = default!;
        public string? PhoneNumber { get; init; } = default!;
        public SubscriptionPlan Plan { get; init; }
    }
}
