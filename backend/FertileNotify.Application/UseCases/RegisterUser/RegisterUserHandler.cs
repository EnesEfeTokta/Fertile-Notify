using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var user = new User(command.Email);

            int monthlyLimit = command.Plan switch
            {
                SubscriptionPlan.Free => 10,
                SubscriptionPlan.Pro => 100,
                SubscriptionPlan.Enterprise => 1000,
                _ => throw new ArgumentOutOfRangeException(),
            };

            var subscription = new Subscription(
                user.Id,
                command.Plan,
                monthlyLimit,
                DateTime.UtcNow.AddMonths(1)
            );

            await _userRepository.SaveAsync(user);
            await _subscriptionRepository.SaveAsync(subscription);

            return user.Id;
        }

        public async Task<Guid> HandleAsync(string email)
        {
            var user = new User(email);

            var subscription = new Subscription(
                userId: user.Id,
                plan: SubscriptionPlan.Free,
                monthlyLimit: 10,
                expiresAt: DateTime.UtcNow.AddMonths(1)
            );

            await _userRepository.SaveAsync(user);
            await _subscriptionRepository.SaveAsync(subscription);

            return user.Id;
        }
    }
}
