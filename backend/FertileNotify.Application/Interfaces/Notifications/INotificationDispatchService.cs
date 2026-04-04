namespace FertileNotify.Application.Interfaces.Notifications
{
    public interface INotificationDispatchService
    {
        Task QueueAsync(
            ProcessNotificationMessage message,
            string source,
            CancellationToken cancellationToken = default);
    }
}
