using FertileNotify.Application.Interfaces;
using FertileNotify.Application.UseCases.ProcessEvent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Infrastructure.BackgroundJobs
{
    public class NotificationWorker : BackgroundService
    {
        private readonly INotificationQueue _queue;
        private readonly IServiceProvider _serviceProvider;

        private readonly ILogger<NotificationWorker> _logger;

        public NotificationWorker(
            INotificationQueue queue, 
            IServiceProvider serviceProvider,
            ILogger<NotificationWorker> logger)
        {
            _queue = queue;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[WORKER] Background service has started...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var command = await _queue.DequeueAsync(stoppingToken);

                    _logger.LogInformation(
                        "[WORKER] New job received: {EventType} -> {SubscriberId}", 
                        command.EventType.Name, 
                        command.SubscriberId
                    );

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var handler = scope.ServiceProvider.GetRequiredService<ProcessEventHandler>();
                        await handler.HandleAsync(command);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[WORKER ERROR] An error occurred while processing a job.");
                }
            }
        }
    }
}
