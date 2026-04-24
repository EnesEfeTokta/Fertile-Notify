namespace FertileNotify.Application.UseCases.UpdateApiKeyStatus
{
    public class UpdateApiKeyStatusCommand : IRequest<Unit>
    {
        public Guid SubscriberId { get; set; }
        public Guid ApiKeyId { get; set; }
        public bool IsActive { get; set; }
    }
}
