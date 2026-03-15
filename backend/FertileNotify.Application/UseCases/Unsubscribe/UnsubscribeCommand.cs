using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Application.UseCases.Unsubscribe
{
    public class UnsubscribeCommand
    {
        public Guid SubscriberId { get; set; }
        public string Recipient { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public List<string>? Channels { get; set; }
    }
}
