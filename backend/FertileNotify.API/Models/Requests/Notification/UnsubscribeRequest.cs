namespace FertileNotify.API.Models.Requests
{
    public class UnsubscribeRequest
    {
        public string Recipient { get; set; } = string.Empty;
        public Guid SubscriberId { get; set; }
        public string Token { get; set; } = string.Empty;
        public List<string>? Channels { get; set; }
    }
}