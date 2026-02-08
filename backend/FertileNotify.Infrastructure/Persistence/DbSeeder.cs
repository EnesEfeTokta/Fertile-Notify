using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
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
                    var templates = new List<NotificationTemplate>
                    {
                        // --- AUTH ---
                        NotificationTemplate.CreateGlobal(
                            EventType.SubscriberRegistered,
                            "Welcome to {AppName}!",
                            "Hi {Name}, thank you for joining us. We are excited to have you on board."
                        ),
                        NotificationTemplate.CreateGlobal(
                            EventType.PasswordReset,
                            "Reset Your Password",
                            "Hello {Name}, you requested a password reset. Use this code: {Code}. If you didn't request this, ignore this email."
                        ),
                        NotificationTemplate.CreateGlobal(
                            EventType.EmailVerified,
                            "Email Verified Successfully",
                            "Hi {Name}, your email address has been verified. You can now access all features."
                        ),
                        NotificationTemplate.CreateGlobal(
                            EventType.LoginAlert,
                            "New Login Detected",
                            "Hello {Name}, we detected a new login to your account from {Device} at {Time}."
                        ),

                        // --- E-COMMERCE ---
                        NotificationTemplate.CreateGlobal(
                            EventType.OrderCreated,
                            "Order Confirmation #{OrderId}",
                            "Dear {Name}, thank you for your order. Total amount: {Amount}. We will notify you when it ships."
                        ),
                        NotificationTemplate.CreateGlobal(
                            EventType.OrderShipped,
                            "Your Order Has Shipped! #{OrderId}",
                            "Great news {Name}! Your order is on its way. Tracking Number: {TrackingNumber}."
                        ),
                        NotificationTemplate.CreateGlobal(
                            EventType.OrderDelivered,
                            "Order Delivered",
                            "Hi {Name}, your order #{OrderId} has been delivered. Enjoy!"
                        ),
                        NotificationTemplate.CreateGlobal(
                            EventType.PaymentFailed,
                            "Payment Failed for Order #{OrderId}",
                            "Hello {Name}, we couldn't process your payment. Please update your payment method to proceed."
                        ),

                        // --- GENERAL ---
                        NotificationTemplate.CreateGlobal(
                            EventType.Campaign,
                            "{CampaignTitle}",
                            "Hi {Name}, check out our latest offer: {CampaignDetails}. Don't miss out!"
                        ),
                        NotificationTemplate.CreateGlobal(
                            EventType.MonthlyNewsletter,
                            "{Month} Newsletter",
                            "Hi {Name}, here are the updates for this month: {Updates}."
                        ),
                        NotificationTemplate.CreateGlobal(
                            EventType.SupportTicketUpdated,
                            "Update on Ticket #{TicketId}",
                            "Hello {Name}, your support ticket has been updated. Status: {Status}. Reply: {Message}."
                        )
                    };

                    await context.NotificationTemplates.AddRangeAsync(templates);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}