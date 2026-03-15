namespace FertileNotify.Application.UseCases.ManageChannels
{
    public class ManageChannelsCommand
    {
        public Guid SubscriberId { get; set; }
        public string Channel { get; set; } = string.Empty;
        public bool Enable { get; set; }
    }
}
