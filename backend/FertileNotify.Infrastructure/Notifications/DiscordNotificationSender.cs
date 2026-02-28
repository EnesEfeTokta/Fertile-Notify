using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace FertileNotify.Infrastructure.Notifications
{
    public class DiscordNotificationSender : INotificationSender
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DiscordNotificationSender> _logger;

        public DiscordNotificationSender(
            HttpClient httpClient,
            ILogger<DiscordNotificationSender> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public NotificationChannel Channel => NotificationChannel.Discord;

        public async Task<bool> SendAsync(Guid subscriberId, string recipient, EventType eventType, string subject, string body, IReadOnlyDictionary<string, string>? providerSettings = null)
        {
            try
            {
                if (providerSettings == null || !providerSettings.TryGetValue("DiscordWebhookUrl", out var webhookUrl))
                    return false;

                var payload = new
                {
                    content = $"_**Contact Person: {recipient}**_\n+--------------------------+\n**{subject}**\n{body}"
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(webhookUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("[DISCORD] Send failed: {Error}", error);
                    throw new Exception($"Discord send failed: {response.StatusCode}");
                }
                _logger.LogInformation("[DISCORD] Subscriber: {SubId}, Recipient: {To}", subscriberId, recipient);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "[DISCORD] -> Exception while sending Discord notification to {Recipient} for subscriber {SubscriberId} and event {EventType}", 
                    recipient, subscriberId, eventType);
                return false; 
            }
        }
    }
}
