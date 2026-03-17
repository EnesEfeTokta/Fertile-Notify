using FertileNotify.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Infrastructure.BackgroundJobs
{
    public class LogRetentionWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<LogRetentionWorker> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24);

        public LogRetentionWorker(IServiceProvider serviceProvider, ILogger<LogRetentionWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("LogRetentionWorker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessLogsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during log retention processing.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task ProcessLogsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<INotificationLogRepository>();

            var now = DateTime.UtcNow;
            var anonymizationCutOff = now.AddDays(-7);
            var deletionCutOff = now.AddDays(-30);

            _logger.LogInformation("Starting log retention cleanup. Anonymization < {AnonymizationDate}, Deletion < {DeletionDate}", 
                anonymizationCutOff, deletionCutOff);

            await repository.DeleteLogsOlderThanAsync(deletionCutOff);
            _logger.LogInformation("Completed deletion of logs older than 30 days.");

            var logsToAnonymize = await repository.GetLogsForAnonymizationAsync(anonymizationCutOff);
            
            if (logsToAnonymize.Any())
            {
                foreach (var log in logsToAnonymize)
                {
                    log.AnonymizeContent();
                }

                await repository.UpdateRangeAsync(logsToAnonymize);
                _logger.LogInformation("Anonymized {Count} logs between 7 and 30 days old.", logsToAnonymize.Count);
            }
            else
            {
                _logger.LogInformation("No logs found for anonymization.");
            }
        }
    }
}
