namespace FertileNotify.Application.UseCases.SetChannelSetting
{
    public class SetChannelSettingCommand
    {
        public Guid SubscriberId { get; set; }
        public string Channel { get; set; } = string.Empty;
        public Dictionary<string, string> Settings { get; set; } = new();
    }
}
