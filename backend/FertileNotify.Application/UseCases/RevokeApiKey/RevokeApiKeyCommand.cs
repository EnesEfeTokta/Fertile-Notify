namespace FertileNotify.Application.UseCases.RevokeApiKey
{
    public class RevokeApiKeyCommand : IRequest<Unit>
    {
        public Guid SubscriberId { get; set; }
        public Guid ApiKeyId { get; set; }
    }
}
