namespace FertileNotify.Infrastructure.Persistence
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            var expectedGlobalTemplates = BuildBaseGlobalTemplates();

            var existingGlobalKeys = await context.NotificationTemplates
                .AsNoTracking()
                .Where(t => t.SubscriberId == null)
                .Select(t => new { EventType = t.EventType.Name, Channel = t.Channel.Name })
                .ToListAsync();

            var existingKeySet = existingGlobalKeys
                .Select(k => BuildTemplateKey(k.EventType, k.Channel))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var missingGlobalTemplates = expectedGlobalTemplates
                .Where(template => !existingKeySet.Contains(BuildTemplateKey(template.EventType.Name, template.Channel.Name)))
                .ToList();

            if (missingGlobalTemplates.Count > 0)
            {
                await context.NotificationTemplates.AddRangeAsync(missingGlobalTemplates);
                await context.SaveChangesAsync();
            }
        }

        private static List<NotificationTemplate> BuildBaseGlobalTemplates()
        {
            var globalTemplates = new List<NotificationTemplate>();

            var channels = GetSupportedChannels();
            var templateDefinitions = GetTemplateDefinitionsByEvent();

            foreach (var channel in channels)
            {
                bool isEmail = channel.Equals(NotificationChannel.Email);

                foreach (var eventType in EventCatalog.All)
                {
                    var definition = templateDefinitions.TryGetValue(eventType.Name, out var mapped)
                        ? mapped
                        : BuildFallbackDefinition(eventType);

                    var body = isEmail
                        ? WrapInMjml(definition.EmailBody)
                        : definition.NonEmailBody;

                    globalTemplates.Add(NotificationTemplate.CreateGlobal(
                        definition.Name,
                        definition.Description,
                        eventType,
                        channel,
                        new NotificationContent(definition.Subject, body)
                    ));
                }
            }

            return globalTemplates;
        }

        private static NotificationChannel[] GetSupportedChannels()
        {
            return
            [
                NotificationChannel.Email,
                NotificationChannel.SMS,
                NotificationChannel.Console,
                NotificationChannel.Discord,
                NotificationChannel.Telegram,
                NotificationChannel.WhatsApp,
                NotificationChannel.MSTeams,
                NotificationChannel.FirebasePush,
                NotificationChannel.Slack,
                NotificationChannel.WebPush,
                NotificationChannel.Webhook
            ];
        }

        private static Dictionary<string, TemplateSeedDefinition> GetTemplateDefinitionsByEvent()
        {
            return new Dictionary<string, TemplateSeedDefinition>(StringComparer.OrdinalIgnoreCase)
            {
                [EventType.SubscriberRegistered.Name] = new(
                    "User Welcome Message",
                    "Sent to new subscribers immediately after registration to welcome them to the platform.",
                    "Welcome to {{Name}}!",
                    "Hi {{Name}}, welcome to our service! We're excited to have you on board. To manage this notification, go to this address. {{RecipientsManagerLink}}",
                    "Hi {{Name}}, welcome to our service! We're excited to have you on board. To manage this notification, go to this address. {{RecipientsManagerLink}}"),

                [EventType.PasswordReset.Name] = new(
                    "Password Reset Request",
                    "Contains the security code or link required for a user to reset their forgotten password.",
                    "Reset Your Password",
                    "Hello {{Name}}, use this code to reset your password: <b>{{Code}}</b>. To manage this notification, go to this address. {{RecipientsManagerLink}}",
                    "Hello {{Name}}, your password reset code is: {{Code}}. To manage this notification, go to this address. {{RecipientsManagerLink}}"),

                [EventType.EmailVerified.Name] = new(
                    "Email Verification Success",
                    "Confirmation sent once the user successfully verifies their email address.",
                    "Email Verified Successfully",
                    "Hi {{Name}}, your email address has been verified. You can now access all features. To manage this notification, go to this address. {{RecipientsManagerLink}}",
                    "Hi {{Name}}, your email address has been verified. You can now access all features. To manage this notification, go to this address. {{RecipientsManagerLink}}"),

                [EventType.LoginAlert.Name] = new(
                    "Security Login Alert",
                    "Notifies the user when a login occurs from a new device or unrecognized location.",
                    "New Login Detected",
                    "Hello {{Name}}, we detected a new login to your account from <b>{{Device}}</b> at <b>{{Time}}</b>. To manage this notification, go to this address. {{RecipientsManagerLink}}",
                    "Hello {{Name}}, we detected a new login to your account from {{Device}} at {{Time}}. To manage this notification, go to this address. {{RecipientsManagerLink}}"),

                [EventType.AccountLocked.Name] = new(
                    "Account Locked Alert",
                    "Sent when an account is temporarily locked due to suspicious activity or repeated failed attempts.",
                    "Your Account Has Been Locked",
                    "Hello {{Name}}, your account was temporarily locked for security reasons. Please follow recovery instructions in your dashboard. To manage this notification, go to this address. {{RecipientsManagerLink}}",
                    "Hello {{Name}}, your account was temporarily locked for security reasons. Check your dashboard for recovery instructions. To manage this notification, go to this address. {{RecipientsManagerLink}}"),

                [EventType.OrderCreated.Name] = new(
                    "Order Confirmation",
                    "Sent after a successful purchase, providing the customer with an order summary.",
                    "Order Confirmation #{{OrderId}}",
                    "Dear {{Name}}, thank you for your order. Total amount: <b>{{Amount}}</b>. To manage this notification, go to this address. {{RecipientsManagerLink}}",
                    "Hi {{Name}}, order #{{OrderId}} for {{Amount}} is received. To manage this notification, go to this address. {{RecipientsManagerLink}}"),

                [EventType.OrderShipped.Name] = new(
                    "Shipping Notification",
                    "Alerts the customer when their order has been dispatched and provides tracking info.",
                    "Your Order Has Shipped! #{{OrderId}}",
                    "Great news {{Name}}! Your order is on its way. Tracking: {{TrackingNumber}}. To manage this notification, go to this address. {{RecipientsManagerLink}}",
                    "Hi {{Name}}, order #{{OrderId}} is shipped! Tracking: {{TrackingNumber}}. To manage this notification, go to this address. {{RecipientsManagerLink}}"),

                [EventType.OrderDelivered.Name] = new(
                    "Delivery Confirmation",
                    "Sent to the customer once the courier marks the package as delivered.",
                    "Order Delivered",
                    "Hi {{Name}}, your order #{{OrderId}} has been delivered. Enjoy! To manage this notification, go to this address. {{RecipientsManagerLink}}",
                    "Hi {{Name}}, your order #{{OrderId}} has been delivered. Enjoy! To manage this notification, go to this address. {{RecipientsManagerLink}}"),

                [EventType.OrderCancelled.Name] = new(
                    "Order Cancellation Notice",
                    "Sent when an order is cancelled by the user or system.",
                    "Order Cancelled #{{OrderId}}",
                    "Hi {{Name}}, your order #{{OrderId}} has been cancelled. If this is unexpected, contact support. To manage this notification, go to this address. {{RecipientsManagerLink}}",
                    "Hi {{Name}}, your order #{{OrderId}} has been cancelled. Contact support if needed. To manage this notification, go to this address. {{RecipientsManagerLink}}"),

                [EventType.PaymentFailed.Name] = new(
                    "Payment Failure Notice",
                    "Urgent notification sent when a transaction fails, asking the user to update payment info.",
                    "Payment Failed for Order #{{OrderId}}",
                    "Hello {{Name}}, we couldn't process your payment. Please update your payment method to proceed. To manage this notification, go to this address. {{RecipientsManagerLink}}",
                    "Hello {{Name}}, we couldn't process your payment. Please update your payment method to proceed. To manage this notification, go to this address. {{RecipientsManagerLink}}"),

                [EventType.SubscriptionRenewed.Name] = new(
                    "Subscription Renewal Confirmation",
                    "Sent after successful subscription renewal including next billing details.",
                    "Subscription Renewed Successfully",
                    "Hi {{Name}}, your subscription has been renewed successfully. Next billing date: <b>{{NextBillingDate}}</b>. To manage this notification, go to this address. {{RecipientsManagerLink}}",
                    "Hi {{Name}}, your subscription has been renewed. Next billing date: {{NextBillingDate}}. To manage this notification, go to this address. {{RecipientsManagerLink}}"),

                [EventType.Campaign.Name] = new(
                    "Marketing Campaign",
                    "A versatile template used for promotional offers, discounts, and announcements.",
                    "{{CampaignTitle}}",
                    "Hi {{Name}}, check out our latest offer: {{CampaignDetails}}. Don't miss out! To manage this notification, go to this address. {{RecipientsManagerLink}}",
                    "Hi {{Name}}, check out our latest offer: {{CampaignDetails}}. Don't miss out! To manage this notification, go to this address. {{RecipientsManagerLink}}"),

                [EventType.MonthlyNewsletter.Name] = new(
                    "Monthly Newsletter",
                    "Sent monthly to keep users informed about platform updates and news.",
                    "{{Month}} Newsletter",
                    "Hi {{Name}}, here are the updates for this month: <b>{{Updates}}</b>. To manage this notification, go to this address. {{RecipientsManagerLink}}",
                    "Hi {{Name}}, here are the updates for this month: {{Updates}}. To manage this notification, go to this address. {{RecipientsManagerLink}}"),

                [EventType.SupportTicketUpdated.Name] = new(
                    "Support Ticket Update",
                    "Notifies the user when there is a new response or status change on their support ticket.",
                    "Update on Ticket #{{TicketId}}",
                    "Hello {{Name}}, your support ticket has been updated. <b>Status: {{Status}}. Reply: {{Message}}.</b> To manage this notification, go to this address. {{RecipientsManagerLink}}",
                    "Hello {{Name}}, your support ticket has been updated. Status: {{Status}}. Reply: {{Message}}. To manage this notification, go to this address. {{RecipientsManagerLink}}"),

                [EventType.TestForDevelop.Name] = new(
                    "Test Event Notification",
                    "Template used for development and pipeline verification scenarios.",
                    "Test Notification",
                    "Hi {{Name}}, this is a development test notification. Context: <b>{{Context}}</b>. To manage this notification, go to this address. {{RecipientsManagerLink}}",
                    "Hi {{Name}}, this is a development test notification. Context: {{Context}}. To manage this notification, go to this address. {{RecipientsManagerLink}}")
            };
        }

        private static TemplateSeedDefinition BuildFallbackDefinition(EventType eventType)
        {
            var title = eventType.Name;

            return new TemplateSeedDefinition(
                $"{title} Notification",
                $"Default template for {title} events.",
                $"{title} Alert",
                "Hi {{Name}}, an event was triggered: <b>" + title + "</b>. To manage this notification, go to this address. {{RecipientsManagerLink}}",
                "Hi {{Name}}, an event was triggered: " + title + ". To manage this notification, go to this address. {{RecipientsManagerLink}}"
            );
        }

        private static string BuildTemplateKey(string eventType, string channel)
            => $"{eventType}|{channel}";

        private sealed record TemplateSeedDefinition(
            string Name,
            string Description,
            string Subject,
            string EmailBody,
            string NonEmailBody
        );

        private static string WrapInMjml(string content)
        {
            return $@"<mjml>
                        <mj-body>
                            <mj-section>
                                <mj-column>
                                    <mj-text font-size=""16px"" font-family=""helvetica"">
                                        {content}
                                    </mj-text>
                                </mj-column>
                            </mj-section>
                        </mj-body>
                      </mjml>";
        }
    }
}