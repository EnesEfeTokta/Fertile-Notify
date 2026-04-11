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

            var channels = new[] {
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
            };

            foreach (var channel in channels)
            {
                bool isEmail = channel.Equals(NotificationChannel.Email);

                // --- AUTH EVENTS ---
                globalTemplates.Add(NotificationTemplate.CreateGlobal(
                    "User Welcome Message",
                    "Sent to new subscribers immediately after registration to welcome them to the platform.",
                    EventType.SubscriberRegistered,
                    channel,
                    new NotificationContent(
                        "Welcome to {{Name}}!",
                        isEmail ? WrapInMjml("Hi {{Name}}, welcome to our service! We're excited to have you on board. To manage this notification, go to this address. {{RecipientsManagerLink}}")
                                : "Hi {{Name}}, welcome to our service! We're excited to have you on board. To manage this notification, go to this address. {{RecipientsManagerLink}}"
                    )
                ));

                globalTemplates.Add(NotificationTemplate.CreateGlobal(
                    "Password Reset Request",
                    "Contains the security code or link required for a user to reset their forgotten password.",
                    EventType.PasswordReset,
                    channel,
                    new NotificationContent(
                        "Reset Your Password",
                        isEmail ? WrapInMjml("Hello {{Name}}, use this code to reset your password: <b>{{Code}}</b>. To manage this notification, go to this address. {{RecipientsManagerLink}}")
                                : "Hello {{Name}}, your password reset code is: {{Code}}. To manage this notification, go to this address. {{RecipientsManagerLink}}"
                    )
                ));

                globalTemplates.Add(NotificationTemplate.CreateGlobal(
                    "Email Verification Success",
                    "Confirmation sent once the user successfully verifies their email address.",
                    EventType.EmailVerified,
                    channel,
                    new NotificationContent(
                        "Email Verified Successfully",
                        isEmail ? WrapInMjml("Hi {{Name}}, your email address has been verified. You can now access all features. To manage this notification, go to this address. {{RecipientsManagerLink}}")
                                : "Hi {{Name}}, your email address has been verified. You can now access all features. To manage this notification, go to this address. {{RecipientsManagerLink}}"
                    )
                ));

                globalTemplates.Add(NotificationTemplate.CreateGlobal(
                    "Security Login Alert",
                    "Notifies the user when a login occurs from a new device or unrecognized location.",
                    EventType.LoginAlert,
                    channel,
                    new NotificationContent(
                        "New Login Detected",
                        isEmail ? WrapInMjml("Hello {{Name}}, we detected a new login to your account from <b>{{Device}}</b> at <b>{{Time}}</b>. To manage this notification, go to this address. {{RecipientsManagerLink}}")
                                : "Hello {{Name}}, we detected a new login to your account from {{Device}} at {{Time}}. To manage this notification, go to this address. {{RecipientsManagerLink}}"
                    )
                ));

                // --- E-COMMERCE EVENTS ---
                globalTemplates.Add(NotificationTemplate.CreateGlobal(
                    "Order Confirmation",
                    "Sent after a successful purchase, providing the customer with an order summary.",
                    EventType.OrderCreated,
                    channel,
                    new NotificationContent(
                        "Order Confirmation #{{OrderId}}",
                        isEmail ? WrapInMjml("Dear {{Name}}, thank you for your order. Total amount: <b>{{Amount}}</b>. To manage this notification, go to this address. {{RecipientsManagerLink}}")
                                : "Hi {{Name}}, order #{{OrderId}} for {{Amount}} is received. To manage this notification, go to this address. {{RecipientsManagerLink}}"
                    )
                ));

                globalTemplates.Add(NotificationTemplate.CreateGlobal(
                    "Shipping Notification",
                    "Alerts the customer when their order has been dispatched and provides tracking info.",
                    EventType.OrderShipped,
                    channel,
                    new NotificationContent(
                        "Your Order Has Shipped! #{{OrderId}}",
                        isEmail ? WrapInMjml("Great news {{Name}}! Your order is on its way. Tracking: {{TrackingNumber}}. To manage this notification, go to this address. {{RecipientsManagerLink}}")
                                : "Hi {{Name}}, order #{{OrderId}} is shipped! Tracking: {{TrackingNumber}}. To manage this notification, go to this address. {{RecipientsManagerLink}}"
                    )
                ));

                globalTemplates.Add(NotificationTemplate.CreateGlobal(
                    "Delivery Confirmation",
                    "Sent to the customer once the courier marks the package as delivered.",
                    EventType.OrderDelivered,
                    channel,
                    new NotificationContent(
                        "Order Delivered",
                        isEmail ? WrapInMjml("Hi {{Name}}, your order #{{OrderId}} has been delivered. Enjoy! To manage this notification, go to this address. {{RecipientsManagerLink}}")
                                : "Hi {{Name}}, your order #{{OrderId}} has been delivered. Enjoy! To manage this notification, go to this address. {{RecipientsManagerLink}}"
                    )
                ));

                globalTemplates.Add(NotificationTemplate.CreateGlobal(
                    "Payment Failure Notice",
                    "Urgent notification sent when a transaction fails, asking the user to update payment info.",
                    EventType.PaymentFailed,
                    channel,
                    new NotificationContent(
                        "Payment Failed for Order #{{OrderId}}",
                        isEmail ? WrapInMjml("Hello {{Name}}, we couldn't process your payment. Please update your payment method to proceed. To manage this notification, go to this address. {{RecipientsManagerLink}}")
                                : "Hello {{Name}}, we couldn't process your payment. Please update your payment method to proceed. To manage this notification, go to this address. {{RecipientsManagerLink}}"
                    )
                ));

                // --- GENERAL EVENTS ---
                globalTemplates.Add(NotificationTemplate.CreateGlobal(
                    "Marketing Campaign",
                    "A versatile template used for promotional offers, discounts, and announcements.",
                    EventType.Campaign,
                    channel,
                    new NotificationContent(
                        "{{CampaignTitle}}",
                        isEmail ? WrapInMjml("Hi {{Name}}, check out our latest offer: {{CampaignDetails}}. Don't miss out! To manage this notification, go to this address. {{RecipientsManagerLink}}")
                                : "Hi {{Name}}, check out our latest offer: {{CampaignDetails}}. Don't miss out! To manage this notification, go to this address. {{RecipientsManagerLink}}"
                    )
                ));

                globalTemplates.Add(NotificationTemplate.CreateGlobal(
                    "Monthly Newsletter",
                    "Sent monthly to keep users informed about platform updates and news.",
                    EventType.MonthlyNewsletter,
                    channel,
                    new NotificationContent(
                        "{{Month}} Newsletter",
                        isEmail ? WrapInMjml("Hi {{Name}}, here are the updates for this month: <b>{{Updates}}.</b> To manage this notification, go to this address. {{RecipientsManagerLink}}")
                                : "Hi {{Name}}, here are the updates for this month: {{Updates}}. To manage this notification, go to this address. {{RecipientsManagerLink}}"
                    )
                ));

                globalTemplates.Add(NotificationTemplate.CreateGlobal(
                    "Support Ticket Update",
                    "Notifies the user when there is a new response or status change on their support ticket.",
                    EventType.SupportTicketUpdated,
                    channel,
                    new NotificationContent(
                        "Update on Ticket #{{TicketId}}",
                        isEmail ? WrapInMjml("Hello {{Name}}, your support ticket has been updated. <b>Status: {{Status}}. Reply: {{Message}}.</b> To manage this notification, go to this address. {{RecipientsManagerLink}}")
                                : "Hello {{Name}}, your support ticket has been updated. Status: {{Status}}. Reply: {{Message}}. To manage this notification, go to this address. {{RecipientsManagerLink}}"
                    )
                ));
            }

            return globalTemplates;
        }

        private static string BuildTemplateKey(string eventType, string channel)
            => $"{eventType}|{channel}";

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