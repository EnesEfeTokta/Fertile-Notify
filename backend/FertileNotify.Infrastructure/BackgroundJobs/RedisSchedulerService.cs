using StackExchange.Redis;

namespace FertileNotify.Infrastructure.BackgroundJobs
{
    public class RedisSchedulerService
    {
        private readonly IDatabase _db;
        private const string Key = "scheduled_workflows";

        public RedisSchedulerService(IConnectionMultiplexer redis) => _db = redis.GetDatabase();

        public async Task ScheduleAsync(Guid workflowId, DateTime nextRun)
        {
            await _db.SortedSetAddAsync(Key, workflowId.ToString(), nextRun.Ticks);
        }

        public async Task<List<Guid>> GetDueWorkflowsAsync()
        {
            var values = await _db.SortedSetRangeByScoreAsync(Key, 0, DateTime.UtcNow.Ticks);
            return values.Select(v => Guid.Parse(v.ToString())).ToList();
        }

        public async Task RemoveAsync(Guid workflowId)
        {
            await _db.SortedSetRemoveAsync(Key, workflowId.ToString());
        }
    }
}
