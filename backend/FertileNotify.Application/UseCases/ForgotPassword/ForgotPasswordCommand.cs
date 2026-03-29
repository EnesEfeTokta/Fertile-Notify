namespace FertileNotify.Application.UseCases.ForgotPassword
{
    public class ForgotPasswordCommand: IRequest<Unit>
    {
        public string Email { get; init; } = string.Empty;
    }
}