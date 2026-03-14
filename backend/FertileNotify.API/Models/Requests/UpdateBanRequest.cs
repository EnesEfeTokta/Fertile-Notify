namespace FertileNotify.API.Models.Requests
{
    public class UpdateBanRequest
    {
        public List<string> Channels { get; set; } = new();
        public bool IsActive { get; set; }
    }
}
