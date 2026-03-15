using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Application.UseCases.SetChannelSetting
{
    public class SetChannelSettingHandler
    {
        private readonly ISubscriberChannelRepository _subscriberChannelRepository;

        public SetChannelSettingHandler(ISubscriberChannelRepository subscriberChannelRepository)
        {
            _subscriberChannelRepository = subscriberChannelRepository;
        }

        public async Task HandleAsync(SetChannelSettingCommand command)
        {
            var channel = NotificationChannel.From(command.Channel);
            var channelSetting = new SubscriberChannelSetting(command.SubscriberId, channel, command.Settings);

            await _subscriberChannelRepository.SaveAsync(channelSetting);
        }
    }
}
