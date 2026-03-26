using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace FertileNotify.Infrastructure.Notifications
{
    public class EmailNotificationSender : INotificationSender
    {
        private readonly ILogger<EmailNotificationSender> _logger;

        public EmailNotificationSender(ILogger<EmailNotificationSender> logger)
        {
            _logger = logger;
        }

        public NotificationChannel Channel => NotificationChannel.Email;

        public async Task<bool> SendAsync(
            Guid subscriberId, 
            string recipient, 
            EventType eventType, 
            NotificationContent content, 
            IReadOnlyDictionary<string, string>? providerSettings = null)
        {
            try
            {
                if (providerSettings == null ||
                    !providerSettings.TryGetValue("SMTP_Host", out var host) ||
                    !providerSettings.TryGetValue("SMTP_Port", out var port) ||
                    !providerSettings.TryGetValue("SMTP_Email", out var email) ||
                    !providerSettings.TryGetValue("SMTP_Password", out var password))
                {
                    return false;
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(providerSettings.GetValueOrDefault("SMTP_OwnerName", "Fertile Notify"), email));
                message.To.Add(MailboxAddress.Parse(recipient));
                message.Subject = content.Subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = content.Body };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                await client.ConnectAsync(host, int.Parse(port), SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(email, password.Trim());
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("[EMAIL] Sent successfully via MailKit to {Recipient}", recipient);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[MAILKIT ERROR] Sending failed for {SubId}", subscriberId);
                return false;
            }
        }
    }
}