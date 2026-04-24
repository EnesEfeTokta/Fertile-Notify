namespace FertileNotify.Application.UseCases.UpdateApiKeyScopes
{
    public class UpdateApiKeyScopesCommand : IRequest<Unit>
    {
        public Guid SubscriberId { get; set; }
        public Guid ApiKeyId { get; set; }
        public string? Scopes { get; set; }
    }
}
