namespace FertileNotify.Application.UseCases.Login
{
    public class LoginCommand: IRequest<Unit>
    {
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
}
