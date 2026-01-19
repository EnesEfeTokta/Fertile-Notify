using FertileNotify.Domain.Enums;

namespace FertileNotify.Application.UseCases.CreateUserWithSubscription
{
    public class CreateUserCommand
    {
        public  string Email { get; set; } = string.Empty;
        public SubscriptionPlan Plan { get; set; }
    }
}
