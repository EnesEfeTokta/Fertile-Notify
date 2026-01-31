namespace FertileNotify.API.Models
{
    public class UpdatePasswordRequest
    {
        public string CurrentPassword { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
    }
}
