using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Application.UseCases.RegisterUser
{
    public class RegisterUserHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        private readonly ILogger<RegisterUserHandler> _logger;

        public RegisterUserHandler(
            IUserRepository userRepository,
            ISubscriptionRepository subscriptionRepository,
            ILogger<RegisterUserHandler> logger
        )
        {
            _userRepository = userRepository;
            _subscriptionRepository = subscriptionRepository;
            _logger = logger;
        }

        public async Task<Guid> HandleAsync(RegisterUserCommand command)
        {
            _logger.LogInformation(
                "User registration is underway. User: {Email} & {PhoneNumber}, Plan: {Plan}", 
                command.Email, 
                string.IsNullOrEmpty(command.PhoneNumber?.Value) ? command.PhoneNumber?.Value : "[No Phone Number]", 
                command.Email.Value
            );

            var user = new User(
                EmailAddress.Create(command.Email.Value), 
                PhoneNumber.Create(command.PhoneNumber?.Value ?? string.Empty)
            );

            var subscription = Subscription.Create(user.Id, command.Plan);

            await _userRepository.SaveAsync(user);
            await _subscriptionRepository.SaveAsync(user.Id, subscription);

            return user.Id;
        }
    }
}