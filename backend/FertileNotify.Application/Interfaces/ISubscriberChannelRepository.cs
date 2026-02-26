using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Application.Interfaces
{
    public interface ISubscriberChannelRepository
    {
        Task SaveAsync(SubscriberChannelSetting setting);
        Task<SubscriberChannelSetting?> GetSettingAsync(Guid subscriberId, NotificationChannel channel);
    }
}
