namespace FertileNotify.Application.UseCases.RevokeApiKey
{
    public class RevokeApiKeyCommand
    {
        public Guid SubscriberId { get; set; }
        public Guid ApiKeyId { get; set; }
    }
}
