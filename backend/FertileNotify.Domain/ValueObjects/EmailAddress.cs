using System.Text.RegularExpressions;

namespace FertileNotify.Domain.ValueObjects
{
    public sealed partial class EmailAddress
    {
        public string Value { get; }

        public EmailAddress(string value)
        {
            Value = value;
        }

        public static EmailAddress Create(string email)
        {
            email = email.Trim();

            if (string.IsNullOrWhiteSpace(email)) 
                throw new ArgumentException("Email cannot be empty.");

            if (!IsValid(email))
                throw new ArgumentException("Email format is invalid.");

            return new EmailAddress(email.Trim().ToLower());
        }

        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase)]
        private static partial Regex EmailRegex();

        private static bool IsValid(string email)
            => EmailRegex().IsMatch(email);

        public override string ToString() => Value;

        public override bool Equals(object? obj)
             => obj is EmailAddress other && Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();
    }
}