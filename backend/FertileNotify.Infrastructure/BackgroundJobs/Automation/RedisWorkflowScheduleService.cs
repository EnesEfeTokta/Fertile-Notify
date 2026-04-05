namespace FertileNotify.Infrastructure.BackgroundJobs.Automation
{
    public class RedisWorkflowScheduleService : IWorkflowScheduleService
    {
        private readonly RedisSchedulerService _redisSchedulerService;

        public RedisWorkflowScheduleService(RedisSchedulerService redisSchedulerService)
        {
            _redisSchedulerService = redisSchedulerService;
        }

        public async Task SyncAsync(AutomationWorkflow workflow)
        {
            await RemoveAsync(workflow.Id);

            if (!workflow.IsActive || string.IsNullOrWhiteSpace(workflow.CronExpression))
            {
                return;
            }

            var nextRun = AutomationSchedulerService.GetNextRun(workflow.CronExpression);
            if (nextRun.HasValue)
            {
                await _redisSchedulerService.ScheduleAsync(workflow.Id, nextRun.Value);
            }
        }

        public async Task RemoveAsync(Guid workflowId)
        {
            await _redisSchedulerService.RemoveAsync(workflowId);
        }
    }
}