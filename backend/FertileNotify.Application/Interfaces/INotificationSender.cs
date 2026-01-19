namespace FertileNotify.Application.Interfaces
{
    public interface INotificationSender
    {
        Task SendAsync(string eventType, string payload);
    }
}