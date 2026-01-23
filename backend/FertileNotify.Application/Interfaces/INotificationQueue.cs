using FertileNotify.Application.UseCases.ProcessEvent;

namespace FertileNotify.Application.Interfaces
{
    public interface INotificationQueue
    {
        ValueTask QueueBackgroundWorkItemAsync(ProcessEventCommand workItem);
        ValueTask<ProcessEventCommand> DequeueAsync(CancellationToken cancellationToken);
    }
}
