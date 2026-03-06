using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WebPush;

namespace FertileNotify.Infrastructure.Notifications
{
    public class WebPushNotificationSender : INotificationSender
    {
        private readonly ILogger<WebPushNotificationSender> _logger;

        public WebPushNotificationSender(ILogger<WebPushNotificationSender> logger)
        {
            _logger = logger;
        }

        public NotificationChannel Channel => NotificationChannel.WebPush;

        public async Task<bool> SendAsync(Guid subscriberId, string recipient, EventType eventType, string subject, string body, IReadOnlyDictionary<string, string>? providerSettings = null)
        {
            try
            {
                if (providerSettings == null ||
                    !providerSettings.TryGetValue("WebPush_VapidPublicKey", out var publicKey) ||
                    !providerSettings.TryGetValue("WebPush_VapidPrivateKey", out var privateKey) ||
                    !providerSettings.TryGetValue("WebPush_OwnerEmail", out var ownerEmail) ||
                    !providerSettings.TryGetValue("WebPush_Icon", out var icon) ||
                    !providerSettings.TryGetValue("WebPush_WebUrl", out var url))
                {
                    _logger.LogWarning("[WEB PUSH] VAPID keys not found for subscriber {SubId}", subscriberId);
                    return false;
                }

                using var doc = JsonDocument.Parse(recipient);
                var root = doc.RootElement;

                var endpoint = root.GetProperty("endpoint").GetString();
                var p256dh = root.GetProperty("keys").GetProperty("p256dh").GetString();
                var auth = root.GetProperty("keys").GetProperty("auth").GetString();

                if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(p256dh) || string.IsNullOrEmpty(auth))
                {
                    _logger.LogError("[WEB PUSH] Invalid subscription format.");
                    return false;
                }

                var subscription = new PushSubscription(endpoint, p256dh, auth);

                var vapidDetails = new VapidDetails($"mailto:{ownerEmail}", publicKey, privateKey);

                var payloadJson = JsonSerializer.Serialize(new
                {
                    title = subject,
                    message = body,
                    icon,
                    url
                });

                var webPushClient = new WebPushClient();
                await webPushClient.SendNotificationAsync(subscription, payloadJson, vapidDetails);

                _logger.LogInformation("[WEB PUSH] Sent successfully to Subscriber: {SubId}", subscriberId);
                return true;
            }
            catch (WebPushException ex)
            {
                _logger.LogError(ex, "[WEB PUSH] Push failed. Status: {Status}", ex.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[WEB PUSH] Unexpected error in WebPushNotificationSender");
                return false;
            }
        }
    }
}