using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;

namespace FertileNotify.Infrastructure.Notifications
{
    public class WhatsAppNotificationSender : INotificationSender
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WhatsAppNotificationSender> _logger;

        public WhatsAppNotificationSender(HttpClient httpClient, ILogger<WhatsAppNotificationSender> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public NotificationChannel Channel => NotificationChannel.WhatsApp;

        public async Task<bool> SendAsync(Guid subscriberId, string recipient, EventType eventType, string subject, string body, IReadOnlyDictionary<string, string>? providerSettings = null)
        {
            try
            {
                if (providerSettings == null ||
                    !providerSettings.TryGetValue("TwilioSid", out var sid) ||
                    !providerSettings.TryGetValue("TwilioToken", out var token) ||
                    !providerSettings.TryGetValue("TwilioFrom", out var fromNumber))
                {
                    return false;
                }

                var url = $"https://api.twilio.com/2010-04-01/Accounts/{sid}/Messages.json";

                var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{sid}:{token}"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("To", $"whatsapp:{recipient}"),
                    new KeyValuePair<string, string>("From", $"whatsapp:{fromNumber}"),
                    new KeyValuePair<string, string>("Body", $"*{subject}*\n{body}")
                });

                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("[WHATSAPP] Send failed: {Error}", error);
                    throw new Exception($"WhatsApp send failed: {response.StatusCode}");
                }

                _logger.LogInformation("[WHATSAPP] Subscriber: {SubId}, Recipient: {To}", subscriberId, recipient);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[WHATSAPP] -> Exception while sending WhatsApp notification to {Recipient} for subscriber {SubscriberId} and event {EventType}",
                    recipient, subscriberId, eventType);
                return false;
            }
        }
    }
}