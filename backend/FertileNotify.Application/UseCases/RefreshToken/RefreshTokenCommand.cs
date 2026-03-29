namespace FertileNotify.Application.UseCases.RefreshToken
{
    public class RefreshTokenCommand: IRequest<LoginResponseDto>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
