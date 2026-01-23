using FertileNotify.Application.Interfaces;
using FertileNotify.Application.UseCases.ProcessEvent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FertileNotify.Infrastructure.BackgroundJobs
{
    public class NotificationWorker : BackgroundService
    {
        private readonly INotificationQueue _queue;
        private readonly IServiceProvider _serviceProvider;

        public NotificationWorker(INotificationQueue queue, IServiceProvider serviceProvider)
        {
            _queue = queue;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("[WORKER] Background service has started...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var command = await _queue.DequeueAsync(stoppingToken);

                    Console.WriteLine($"[WORKER] New job received: {command.EventType.Name} -> {command.UserId}");

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var handler = scope.ServiceProvider.GetRequiredService<ProcessEventHandler>();
                        await handler.HandleAsync(command);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WORKER ERROR] An error occurred while processing: {ex.Message}");
                }
            }
        }
    }
}
