namespace FertileNotify.Application.UseCases.DeleteApiKey
{
    public class DeleteApiKeyCommand : IRequest<Unit>
    {
        public Guid SubscriberId { get; set; }
        public Guid ApiKeyId { get; set; }
    }
}
