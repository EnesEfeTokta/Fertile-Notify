namespace FertileNotify.API.Models
{
    public class ManageChannelRequest
    {
        public string Channel { get; set; } = string.Empty;
        public bool Enable { get; set; }
    }
}
