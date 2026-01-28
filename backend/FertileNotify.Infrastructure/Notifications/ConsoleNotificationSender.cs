using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Infrastructure.Notifications
{
    public class ConsoleNotificationSender : INotificationSender
    {
        private readonly ILogger<ConsoleNotificationSender> _logger;

        public ConsoleNotificationSender(ILogger<ConsoleNotificationSender> logger)
        {
            _logger = logger;
        }

        public NotificationChannel Channel => NotificationChannel.Console;

        public Task SendAsync(string recipient, string subject, string body)
        {
            _logger.LogInformation(
                "[CONSOLE] Sent to: {Recipient} | Subject: {Subject} | Body: {Body}",
                recipient,
                subject,
                body
            );
            return Task.CompletedTask;
        }
    }
}
