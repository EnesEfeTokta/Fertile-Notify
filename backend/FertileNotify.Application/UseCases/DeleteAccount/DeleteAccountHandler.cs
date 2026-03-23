using System;
using Microsoft.Extensions.Logging;
using FertileNotify.Application.Interfaces;

namespace FertileNotify.Application.UseCases.DeleteAccount
{
    public class DeleteAccountHandler
    {
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly ILogger<DeleteAccountHandler> _logger;

        public DeleteAccountHandler(ISubscriberRepository subscriberRepository, ILogger<DeleteAccountHandler> logger)
        {
            _subscriberRepository = subscriberRepository;
            _logger = logger;
        }

        public async Task HandleAsync(DeleteAccountCommand command)
        {
            _logger.LogInformation("Subscriber with ID {SubscriberId} has been deleted.", command.SubscriberId);
        }
    }
}

