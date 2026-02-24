namespace FertileNotify.API.Models.Requests
{
    public class UpdateContactRequest
    {
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }
}
