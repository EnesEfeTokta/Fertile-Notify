using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Infrastructure.Notifications
{
    public class WebhookNotificationSender : INotificationSender
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WebhookNotificationSender> _logger;

        public WebhookNotificationSender(HttpClient httpClient, ILogger<WebhookNotificationSender> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public NotificationChannel Channel => NotificationChannel.Webhook;

        public async Task<bool> SendAsync(
            Guid subscriberId, 
            string recipient, 
            EventType eventType, 
            NotificationContent content, 
            IReadOnlyDictionary<string, string>? providerSettings = null)
        {
            try
            {
                if (string.IsNullOrEmpty(recipient) || !Uri.IsWellFormedUriString(recipient, UriKind.Absolute))
                {
                    _logger.LogWarning("[WEBHOOK] Invalid URL for subscriber {SubId}: {Url}", subscriberId, recipient);
                    return false;
                }

                var payload = new
                {
                    id = Guid.NewGuid(),
                    subscriber_id = subscriberId,
                    event_type = eventType.Name,
                    subject = content.Subject,
                    message = content.Body,
                    timestamp = DateTime.UtcNow
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                var stringContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                if (providerSettings != null && providerSettings.TryGetValue("Webhook_Secret", out var secret))
                {
                    var signature = GenerateSignature(jsonPayload, secret);
                    _httpClient.DefaultRequestHeaders.Remove("X-Fertile-Signature");
                    _httpClient.DefaultRequestHeaders.Add("X-Fertile-Signature", signature);
                }

                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("FertileNotify-Webhook/1.0");

                var response = await _httpClient.PostAsync(recipient, stringContent);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("[WEBHOOK] Successfully sent to {Url}", recipient);
                    return true;
                }

                _logger.LogWarning("[WEBHOOK] Failed to send to {Url}. Status: {Status}", recipient, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[WEBHOOK] Unexpected error sending to {Url}", recipient);
                return false;
            }
        }

        private string GenerateSignature(string payload, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var payloadBytes = Encoding.UTF8.GetBytes(payload);
            using var hmac = new HMACSHA256(keyBytes);
            var hash = hmac.ComputeHash(payloadBytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}