using System.Text.RegularExpressions;

namespace FertileNotify.Domain.ValueObjects
{
    public sealed class EmailAddress
    {
        public string Value { get; }

        public EmailAddress(string value)
        {
            Value = value;
        }

        public static EmailAddress Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) 
                throw new ArgumentException("Email cannot be empty.");

            if (!IsValid(email))
                throw new ArgumentException("Email format is invalid.");

            return new EmailAddress(email.Trim().ToLower());
        }

        private static bool IsValid(string email)
        {
            return Regex.IsMatch(
                email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase
            );
        }

        public override string ToString() => Value;

        public override bool Equals(object? obj)
             => obj is EmailAddress other && Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();
    }
}