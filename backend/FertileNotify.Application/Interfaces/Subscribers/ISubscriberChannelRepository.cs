namespace FertileNotify.Application.Interfaces.Subscribers
{
    public interface ISubscriberChannelRepository
    {
        Task SaveAsync(SubscriberChannelSetting setting);
        Task<SubscriberChannelSetting?> GetSettingAsync(Guid subscriberId, NotificationChannel channel);
        Task<List<SubscriberChannelSetting>> GetAllSettingsAsync(Guid subscriberId);
    }
}
