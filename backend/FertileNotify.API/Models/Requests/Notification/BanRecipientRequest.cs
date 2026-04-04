namespace FertileNotify.API.Models.Requests
{
    public class BanRecipientRequest
    {
        public string RecipientAddress { get; set; } = string.Empty;
        public List<string> Channels { get; set; } = new();
    }
}
