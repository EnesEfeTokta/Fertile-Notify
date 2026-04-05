using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Infrastructure.Notifications
{
    public class FirebasePushNotificationSender : INotificationSender
    {
        private readonly ILogger<FirebasePushNotificationSender> _logger;

        public FirebasePushNotificationSender(ILogger<FirebasePushNotificationSender> logger)
        {
            _logger = logger;
        }

        public NotificationChannel Channel => NotificationChannel.FirebasePush;

        public async Task<bool> SendAsync(
            Guid subscriberId, 
            string recipient, 
            EventType eventType, 
            NotificationContent content, 
            IReadOnlyDictionary<string, string>? providerSettings = null)
        {
            try
            {
                if (providerSettings == null || !providerSettings.TryGetValue("Firebase_ServiceAccountJson", out var jsonKey))
                {
                    _logger.LogWarning("[FIREBASE] Access Token not found for subscriber {SubId}", subscriberId);
                    return false;
                }

                var appName = subscriberId.ToString();
                FirebaseApp app;

                if (FirebaseApp.GetInstance(appName) == null)
                {
                    app = FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromJson(jsonKey)
                    }, appName);
                }
                else
                {
                    app = FirebaseApp.GetInstance(appName);
                }

                var messaging = FirebaseMessaging.GetMessaging(app);

                var message = new Message()
                {
                    Token = recipient,
                    Notification = new FirebaseAdmin.Messaging.Notification()
                    {
                        Title = content.Subject,
                        Body = content.Body
                    },
                    Data = new Dictionary<string, string>()
                    {
                        { "eventType", eventType.Name }
                    }
                };

                var result = await messaging.SendAsync(message);
                _logger.LogDebug("[FIREBASE] Message sent, result: {Result}", result);

                _logger.LogInformation("[FIREBASE] Subscriber: {SubId}, Recipient: {To}", subscriberId, recipient);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[FIREBASE] -> Exception while sending Firebase Push notification to {Recipient} for subscriber {SubscriberId} and event {EventType}",
                    recipient, subscriberId, eventType);
                return false;
            }
        }
    }
}