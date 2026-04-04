using FertileNotify.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FertileNotify.Application.Services;

namespace FertileNotify.Infrastructure.BackgroundJobs.Automation
{
    public class AutomationWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public AutomationWorker(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var redis = scope.ServiceProvider.GetRequiredService<RedisSchedulerService>();
                    var scheduler = scope.ServiceProvider.GetRequiredService<AutomationSchedulerService>();
                    var repo = scope.ServiceProvider.GetRequiredService<IAutomationRepository>();

                    var dueIds = await redis.GetDueWorkflowsAsync();

                    foreach (var id in dueIds)
                    {
                        await scheduler.TriggerWorkflow(id);
                        await redis.RemoveAsync(id);

                        var workflow = await repo.GetByIdAsync(id);
                        if (!string.IsNullOrEmpty(workflow?.CronExpression))
                        {
                            var next = AutomationSchedulerService.GetNextRun(workflow.CronExpression);
                            if (next.HasValue) await redis.ScheduleAsync(id, next.Value);
                        }
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
