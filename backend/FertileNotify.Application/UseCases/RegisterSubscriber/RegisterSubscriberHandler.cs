using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Rules;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Application.UseCases.RegisterSubscriber
{
    public class RegisterSubscriberHandler
    {
        private readonly ISubscriberRepository _userRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        private readonly ILogger<RegisterSubscriberHandler> _logger;

        public RegisterSubscriberHandler(
            ISubscriberRepository userRepository,
            ISubscriptionRepository subscriptionRepository,
            ILogger<RegisterSubscriberHandler> logger
        )
        {
            _userRepository = userRepository;
            _subscriptionRepository = subscriptionRepository;
            _logger = logger;
        }

        public async Task<Guid> HandleAsync(RegisterSubscriberCommand command)
        {
            _logger.LogInformation(
                "Subscriber registration is underway. Subscriber Name: {CompanyName}, Contact: {Email} & {PhoneNumber}, Plan: {Plan}",
                command.CompanyName,
                command.Email, 
                string.IsNullOrEmpty(command.PhoneNumber?.Value) ? command.PhoneNumber?.Value : "[No Phone Number]", 
                command.Email.Value
            );

            //PasswordRule.EnsureIsStrong(command.Password.Hash); // Mimari bir sorun nedeniyle şimdilik yorum satırına alındı.

            var user = new Subscriber(command.CompanyName, command.Password, command.Email, command.PhoneNumber);

            var subscription = Subscription.Create(user.Id, command.Plan);

            await _userRepository.SaveAsync(user);
            await _subscriptionRepository.SaveAsync(user.Id, subscription);

            return user.Id;
        }
    }
}