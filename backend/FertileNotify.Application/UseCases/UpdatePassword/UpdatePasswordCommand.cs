namespace FertileNotify.Application.UseCases.UpdatePassword
{
    public class UpdatePasswordCommand : IRequest<Unit>
    {
        public Guid SubscriberId { get; set; }
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
