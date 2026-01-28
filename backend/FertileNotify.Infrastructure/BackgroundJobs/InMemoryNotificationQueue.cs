using System.Threading.Channels;
using FertileNotify.Application.Interfaces;
using FertileNotify.Application.UseCases.ProcessEvent;

namespace FertileNotify.Infrastructure.BackgroundJobs
{
    public class InMemoryNotificationQueue : INotificationQueue
    {
        private readonly Channel<ProcessEventCommand> _queue;

        public InMemoryNotificationQueue()
        {
            var options = new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            };
            _queue = Channel.CreateUnbounded<ProcessEventCommand>(options);
        }

        public async ValueTask<ProcessEventCommand> DequeueAsync(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }

        public async ValueTask QueueBackgroundWorkItemAsync(ProcessEventCommand workItem)
        {
            if (workItem == null)
                throw new ArgumentNullException(nameof(workItem));

            await _queue.Writer.WriteAsync(workItem);
        }
    }
}
