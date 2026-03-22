using System;

namespace FertileNotify.Application.UseCases.VerifyCode
{
    public class VerifyCodeCommand
    {
        public string Email { get; init; } = string.Empty;
        public string Code { get; init; } = string.Empty;
    }
}
