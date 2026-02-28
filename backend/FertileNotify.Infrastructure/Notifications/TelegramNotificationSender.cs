using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace FertileNotify.Infrastructure.Notifications
{
    public class TelegramNotificationSender : INotificationSender
    {
        private readonly ILogger<TelegramNotificationSender> _logger;
        private readonly HttpClient _httpClient;

        public TelegramNotificationSender(ILogger<TelegramNotificationSender> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public NotificationChannel Channel => NotificationChannel.Telegram;

        public async Task<bool> SendAsync(Guid subscriberId, string recipient, EventType eventType, string subject, string body, IReadOnlyDictionary<string, string>? providerSettings = null)
        {
            try
            {
                if (providerSettings == null || !providerSettings.TryGetValue("TelegramBotToken", out var botToken))
                    return false;

                var url = $"https://api.telegram.org/bot{botToken}/sendMessage";
                var payload = new { 
                    chat_id = recipient,
                    text = $"*{subject}*\n\n{body}", 
                    parse_mode = "Markdown" 
                };

                var response = await _httpClient.PostAsJsonAsync(url, payload);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Telegram API Error: {Error}", error);
                    throw new Exception($"Telegram send failed: {response.StatusCode}");
                }
                _logger.LogInformation("[TELEGRAM] Subscriber: {SubId}, Recipient: {To}", subscriberId, recipient);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "[TELEGRAM] -> Exception while sending Telegram notification to {Recipient} for subscriber {SubscriberId} and event {EventType}", 
                    recipient, subscriberId, eventType);
                return false;
            }
        }
    }
}