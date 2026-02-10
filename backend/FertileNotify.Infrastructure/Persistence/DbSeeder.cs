using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FertileNotify.Infrastructure.Persistence
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await context.Database.MigrateAsync();

                if (!await context.NotificationTemplates.AnyAsync(t => t.SubscriberId == null))
                {
                    var globalTemplates = new List<NotificationTemplate>();

                    var channels = new[] {
                        NotificationChannel.Email,
                        NotificationChannel.SMS,
                        NotificationChannel.Console
                    };

                    foreach (var channel in channels)
                    {
                        bool isEmail = channel.Equals(NotificationChannel.Email);

                        // --- AUTH EVENTS ---
                        globalTemplates.Add(NotificationTemplate.CreateGlobal(
                            EventType.SubscriberRegistered,
                            channel,
                            "Welcome to {AppName}!",
                            isEmail ? WrapInMjml("Hi {Name}, thank you for joining us. We are excited to have you on board.")
                                    : "Hi {Name}, welcome to {AppName}! We are excited to have you on board."
                        ));

                        globalTemplates.Add(NotificationTemplate.CreateGlobal(
                            EventType.PasswordReset,
                            channel,
                            "Reset Your Password",
                            isEmail ? WrapInMjml("Hello {Name}, use this code to reset your password: <b>{Code}</b>")
                                    : "Hello {Name}, your password reset code is: {Code}"
                        ));

                        globalTemplates.Add(NotificationTemplate.CreateGlobal(
                            EventType.EmailVerified,
                            channel,
                            "Email Verified Successfully",
                            isEmail ? WrapInMjml("Hi {Name}, your email address has been verified. You can now access all features.")
                                    : "Hi {Name}, your email address has been verified. You can now access all features."
                        ));

                        globalTemplates.Add(NotificationTemplate.CreateGlobal(
                            EventType.LoginAlert,
                            channel,
                            "New Login Detected",
                            isEmail ? WrapInMjml("Hello {Name}, we detected a new login to your account from <b>{Device}</b> at <b>{Time}</b>.")
                                    : "Hello {Name}, we detected a new login to your account from {Device} at {Time}."
                        ));

                        // --- E-COMMERCE EVENTS ---
                        globalTemplates.Add(NotificationTemplate.CreateGlobal(
                            EventType.OrderCreated,
                            channel,
                            "Order Confirmation #{OrderId}",
                            isEmail ? WrapInMjml("Dear {Name}, thank you for your order. Total amount: <b>{Amount}</b>.")
                                    : "Hi {Name}, order #{OrderId} for {Amount} is received."
                        ));

                        globalTemplates.Add(NotificationTemplate.CreateGlobal(
                            EventType.OrderShipped,
                            channel,
                            "Your Order Has Shipped! #{OrderId}",
                            isEmail ? WrapInMjml("Great news {Name}! Your order is on its way. Tracking: {TrackingNumber}")
                                    : "Hi {Name}, order #{OrderId} is shipped! Tracking: {TrackingNumber}"
                        ));

                        globalTemplates.Add(NotificationTemplate.CreateGlobal(
                            EventType.OrderDelivered,
                            channel,
                            "Order Delivered",
                            isEmail ? WrapInMjml("Hi {Name}, your order #{OrderId} has been delivered. Enjoy!")
                                    : "Hi {Name}, your order #{OrderId} has been delivered. Enjoy!"
                        ));

                        globalTemplates.Add(NotificationTemplate.CreateGlobal(
                            EventType.PaymentFailed,
                            channel,
                            "Payment Failed for Order #{OrderId}",
                            isEmail ? WrapInMjml("Hello {Name}, we couldn't process your payment. Please update your payment method to proceed.")
                                    : "Hello {Name}, we couldn't process your payment. Please update your payment method to proceed."
                        ));

                        // --- GENERAL EVENTS ---
                        globalTemplates.Add(NotificationTemplate.CreateGlobal(
                            EventType.Campaign,
                            channel,
                            "{CampaignTitle}",
                            isEmail ? WrapInMjml("Hi {Name}, check out our latest offer: {CampaignDetails}. Don't miss out!")
                                    : "Hi {Name}, check out our latest offer: {CampaignDetails}. Don't miss out!"
                        ));

                        globalTemplates.Add(NotificationTemplate.CreateGlobal(
                            EventType.MonthlyNewsletter,
                            channel,
                            "{Month} Newsletter",
                            isEmail ? WrapInMjml("Hi {Name}, here are the updates for this month: <b>{Updates}.</b>")
                                    : "Hi {Name}, here are the updates for this month: {Updates}."
                        ));

                        globalTemplates.Add(NotificationTemplate.CreateGlobal(
                            EventType.SupportTicketUpdated,
                            channel,
                            "Update on Ticket #{TicketId}",
                            isEmail ? WrapInMjml("Hello {Name}, your support ticket has been updated. <b>Status: {Status}. Reply: {Message}.</b>")
                                    : "Hello {Name}, your support ticket has been updated. Status: {Status}. Reply: {Message}."
                        ));
                    }

                    await context.NotificationTemplates.AddRangeAsync(globalTemplates);
                    await context.SaveChangesAsync();
                }
            }
        }

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