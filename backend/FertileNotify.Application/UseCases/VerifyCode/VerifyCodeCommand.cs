namespace FertileNotify.Application.UseCases.VerifyCode
{
    public class VerifyCodeCommand : ICommand<LoginResponseDto>
    {
        public string Email { get; init; } = string.Empty;
        public string Code { get; init; } = string.Empty;
    }
}
