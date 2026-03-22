namespace FertileNotify.Application.UseCases.UpdatePassword
{
    public class UpdatePasswordCommand
    {
        public Guid SubscriberId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
