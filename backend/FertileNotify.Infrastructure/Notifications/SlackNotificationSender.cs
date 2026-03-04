using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FertileNotify.Infrastructure.Notifications
{
    public class SlackNotificationSender : INotificationSender
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SlackNotificationSender> _logger;

        public SlackNotificationSender(HttpClient httpClient, ILogger<SlackNotificationSender> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public NotificationChannel Channel => NotificationChannel.Slack;

        public async Task<bool> SendAsync(Guid subscriberId, string recipient, EventType eventType, string subject, string body, IReadOnlyDictionary<string, string>? providerSettings = null)
        {
            try
            {
                if (providerSettings == null || !providerSettings.TryGetValue("SlackAccessToken", out var token))
                {
                    _logger.LogWarning("[SLACK] Access Token not found for subscriber {SubId}", subscriberId);
                    return false;
                }

                var url = "https://slack.com/api/chat.postMessage";

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var payload = new
                {
                    channel = recipient,
                    text = $"*Subject:* {subject}\n{body}"
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(responseBody);
                if (!doc.RootElement.GetProperty("ok").GetBoolean())
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("[SLACK] Send failed: {Error}", error);
                    throw new Exception($"Slack send failed: {response.StatusCode}");
                }
                _logger.LogInformation("[SLACK] Subscriber: {SubId}, Recipient: {To}", subscriberId, recipient);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[SLACK] -> Exception while sending Slack notification to {Recipient} for subscriber {SubscriberId} and event {EventType}",
                    recipient, subscriberId, eventType);
                return false;
            }
        }
    }
}
