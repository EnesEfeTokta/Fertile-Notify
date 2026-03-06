using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace FertileNotify.Infrastructure.Notifications
{
    public class MSTeamsNotificationSender : INotificationSender
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MSTeamsNotificationSender> _logger;

        public MSTeamsNotificationSender(HttpClient httpClient, ILogger<MSTeamsNotificationSender> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public NotificationChannel Channel => NotificationChannel.MSTeams;

        public async Task<bool> SendAsync(Guid subscriberId, string recipient, EventType eventType, string subject, string body, IReadOnlyDictionary<string, string>? providerSettings = null)
        {
            try
            {
                if (providerSettings == null || !providerSettings.TryGetValue("MSTeams_WebhookUrl", out var url))
                {
                    _logger.LogWarning("[MSTEAMS] Webhook URL not found for subscriber {SubId}", subscriberId);
                    return false;
                }

                var payload = new
                {
                    text = $"### {subject}\n\n{body}"
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("[MSTEAMS] Send failed: {Error}", error);
                    throw new Exception($"MS Teams send failed: {response.StatusCode}");
                }
                _logger.LogInformation("[MSTEAMS] Subscriber: {SubId}, Recipient: {To}", subscriberId, recipient);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[MSTEAMS] -> Exception while sending Microsoft Teams notification to {Recipient} for subscriber {SubscriberId} and event {EventType}",
                    recipient, subscriberId, eventType);
                return false;
            }
        }
    }
}