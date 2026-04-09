namespace FertileNotify.Application.Interfaces.Notifications
{
    public interface ISystemNotificationService
    {
        Task SendAsync(Guid subscriberId, string title, string message, bool sendEmail = false);
    }
}
