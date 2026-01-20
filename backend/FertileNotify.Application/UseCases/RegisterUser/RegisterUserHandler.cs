using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Enums;
using FertileNotify.Domain.Rules;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Application.UseCases.RegisterUser
{
    public class RegisterUserHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public RegisterUserHandler(
            IUserRepository userRepository,
            ISubscriptionRepository subscriptionRepository
        )
        {
            _userRepository = userRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<Guid> HandleAsync(RegisterUserCommand command)
        {
            EmailAddress emailAddress = EmailAddress.Create(command.Email);

            var user = new User(emailAddress);

            int monthlyLimit = command.Plan switch
            {
                SubscriptionPlan.Free => 10,
                SubscriptionPlan.Pro => 100,
                SubscriptionPlan.Enterprise => 1000,
                _ => throw new ArgumentOutOfRangeException(),
            };

            var allowedEvents = SubscriptionEventPolicy.GetAllowedEvents(command.Plan);

            var subscription = new Subscription(
                user.Id,
                command.Plan,
                monthlyLimit,
                DateTime.UtcNow.AddMonths(1),
                allowedEvents
            );

            await _userRepository.SaveAsync(user);
            await _subscriptionRepository.SaveAsync(user.Id, subscription);

            return user.Id;
        }
    }
}
