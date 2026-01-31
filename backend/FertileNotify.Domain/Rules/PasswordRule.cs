using FertileNotify.Domain.Exceptions;

namespace FertileNotify.Domain.Rules
{
    public static class PasswordRule
    {
        public static void EnsureIsStrong(string password)
        {
            if (password.Length < 8)
                throw new BusinessRuleException("Password must be at least 8 characters long.");

            if (!password.Any(char.IsUpper))
                throw new BusinessRuleException("Password must contain at least one uppercase letter.");

            if (!password.Any(char.IsLower))
                throw new BusinessRuleException("Password must contain at least one lowercase letter.");

            if (!password.Any(char.IsDigit))
                throw new BusinessRuleException("Password must contain at least one digit.");

            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
                throw new BusinessRuleException("Password must contain at least one special character.");
        }
    }
}
