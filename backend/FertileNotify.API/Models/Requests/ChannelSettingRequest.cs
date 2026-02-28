namespace FertileNotify.API.Models.Requests
{
    public class ChannelSettingRequest
    {
        public string Channel { get; set; } = null!;
        public Dictionary<string, string> Settings { get; set; } = new();
    }
}
