namespace FertileNotify.Application.UseCases.Unsubscribe
{
    public class UnsubscribeCommand : IRequest<Unit>
    {
        public Guid SubscriberId { get; set; }
        public string Recipient { get; set; } = string.Empty;
        public List<string>? Channels { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
