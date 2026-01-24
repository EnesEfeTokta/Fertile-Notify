using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
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
            EmailAddress emailAddress = EmailAddress.Create(command.Email.Value);
            
            var user = new User(emailAddress, new PhoneNumber("000-000-00-00"));

            var subscription = Subscription.Create(user.Id, command.Plan);

            await _userRepository.SaveAsync(user);
            await _subscriptionRepository.SaveAsync(user.Id, subscription);

            return user.Id;
        }
    }
}