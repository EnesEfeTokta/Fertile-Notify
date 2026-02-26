using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace FertileNotify.Infrastructure.Notifications
{
    public class TelegramNotificationSender : INotificationSender
    {
        private readonly ILogger _logger;
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
                if (providerSettings == null || !providerSettings.TryGetValue("BotToken", out var botToken))
                    return false;

                var url = $"https://api.telegram.org/bot{botToken}/sendMessage";
                var payload = new { 
                    chat_id = recipient, 
                    text = $"*{subject}*\n\n{body}", 
                    parse_mode = "Markdown" 
                };

                var response = await _httpClient.PostAsJsonAsync(url, payload);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
    }
}
