namespace FertileNotify.Application.DTOs.Security
{
    public class LoginResponseDto
    {
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;

        public LoginResponseDto(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
