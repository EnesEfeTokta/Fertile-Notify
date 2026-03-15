namespace FertileNotify.Application.UseCases.CreateApiKey
{
    public class CreateApiKeyCommand
    {
        public Guid SubscriberId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
