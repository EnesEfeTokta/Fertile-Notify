using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Enums;
using FertileNotify.Domain.Rules;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Application.UseCases.CreateUserWithSubscription
{
    public class CreateUserHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public CreateUserHandler(IUserRepository userRepository, ISubscriptionRepository subscriptionRepository)
        {
            _userRepository = userRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<Guid> HandleAsync(CreateUserCommand command)
        {
            EmailAddress emailAddress = EmailAddress.Create(command.Email);

            var user = new User(emailAddress);
            await _userRepository.SaveAsync(user);

            var allowedEvents = SubscriptionEventPolicy.GetAllowedEvents(command.Plan);

            var subscription = new Subscription(
                user.Id,
                command.Plan,
                command.Plan == SubscriptionPlan.Free ? 10 : 100,
                DateTime.UtcNow.AddMonths(1),
                allowedEvents
            );
            await _subscriptionRepository.SaveAsync(user.Id, subscription);

            return user.Id;
        }
    }
}
