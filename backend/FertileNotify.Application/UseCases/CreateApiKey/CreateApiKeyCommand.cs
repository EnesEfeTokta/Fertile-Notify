namespace FertileNotify.Application.UseCases.CreateApiKey
{
    public class CreateApiKeyCommand : IRequest<string>
    {
        public Guid SubscriberId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
