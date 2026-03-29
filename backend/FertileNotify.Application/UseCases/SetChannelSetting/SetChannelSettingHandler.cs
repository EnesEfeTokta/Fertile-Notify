namespace FertileNotify.Application.UseCases.SetChannelSetting
{
    public class SetChannelSettingHandler : IRequestHandler<SetChannelSettingCommand, Unit>
    {
        private readonly ISubscriberChannelRepository _subscriberChannelRepository;

        public SetChannelSettingHandler(ISubscriberChannelRepository subscriberChannelRepository)
        {
            _subscriberChannelRepository = subscriberChannelRepository;
        }

        public async Task<Unit> Handle(SetChannelSettingCommand command, CancellationToken cancellationToken)
        {
            var channel = NotificationChannel.From(command.Channel);
            var setting = new SubscriberChannelSetting(command.SubscriberId, channel, command.Settings);
            await _subscriberChannelRepository.SaveAsync(setting);
            return Unit.Value;
        }
    }
}
