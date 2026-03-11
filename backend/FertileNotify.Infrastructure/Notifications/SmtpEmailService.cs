using FertileNotify.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace FertileNotify.Infrastructure.Notifications
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var host = _configuration["SystemSmtp:Host"];
            var port = int.Parse(_configuration["SystemSmtp:Port"] ?? "587");
            var username = _configuration["SystemSmtp:Username"];
            var password = _configuration["SystemSmtp:Password"];
            var from = _configuration["SystemSmtp:From"];
            var displayName = _configuration["SystemSmtp:DisplayName"] ?? username;

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(displayName, from!));
                message.To.Add(MailboxAddress.Parse(to));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = body };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(username, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("[EMAIL] Sent successfully via MailKit to {Recipient}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError("[EMAIL] Failed to send email to {Recipient}: {Error}", to, ex.Message);
            }
        }
    }
}