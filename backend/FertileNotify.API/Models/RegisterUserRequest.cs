using FertileNotify.Domain.Enums;

namespace FertileNotify.API.Models
{
    public class RegisterUserRequest
    {
        public string Email { get; init; } = string.Empty;
        public string? PhoneNumber { get; init; } = string.Empty;
        public string Plan { get; init; } = string.Empty;
    }
}
