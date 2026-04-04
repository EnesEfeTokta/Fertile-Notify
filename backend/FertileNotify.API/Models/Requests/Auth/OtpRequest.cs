namespace FertileNotify.API.Models.Requests
{
    public class OtpRequest
    {
        public string Email { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
    }
}
