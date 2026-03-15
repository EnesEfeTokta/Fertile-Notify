using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Application.UseCases.Unsubscribe
{
    public class UnsubscribeHandler
    {
        private readonly IBlacklistRepository _blacklistRepository;
        private readonly ISecurityService _securityService;

        public UnsubscribeHandler(IBlacklistRepository blacklistRepository, ISecurityService securityService)
        {
            _blacklistRepository = blacklistRepository;
            _securityService = securityService;
        }

        public async Task<bool> HandleAsync(UnsubscribeCommand command)
        {
            bool isValid = _securityService.VerifyUnsubscribeToken(command.Recipient, command.SubscriberId, command.Token);

            if (!isValid)
                return false;

            var forbiddenRecipient = new ForbiddenRecipient(
                command.SubscriberId,
                command.Recipient,
                command.Channels?.Select(NotificationChannel.From).ToList() ?? new List<NotificationChannel>()
            );
            await _blacklistRepository.AddOrUpdateAsync(forbiddenRecipient);

            return true;
        }
    }
}
