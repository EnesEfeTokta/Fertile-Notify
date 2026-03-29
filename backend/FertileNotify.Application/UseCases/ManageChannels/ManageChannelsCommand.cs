namespace FertileNotify.Application.UseCases.ManageChannels
{
    public class ManageChannelsCommand : IRequest<Unit>
    {
        public Guid SubscriberId { get; set; }
        public string Channel { get; set; } = string.Empty;
        public bool Enable { get; set; }
    }
}
