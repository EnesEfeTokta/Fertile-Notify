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

                if (!await context.NotificationTemplates.AnyAsync())
                {
                    var templates = new List<NotificationTemplate>
                    {
                        NotificationTemplate.Create(
                            EventType.SubscriberRegistered,
                            "Welcome {Name}!",
                            "Hello {Name}, thank you for joining the FertileNotify family. Your email address is: {Email}"
                        ),
                        NotificationTemplate.Create(
                            EventType.OrderCreated,
                            "Your order has been received #{OrderId}",
                            "Dear {CustomerName}, your order for {Amount} has been processed."
                        )
                    };

                    await context.NotificationTemplates.AddRangeAsync(templates);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}