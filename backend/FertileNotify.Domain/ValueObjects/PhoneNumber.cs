using System.Text.RegularExpressions;

namespace FertileNotify.Domain.ValueObjects
{
    public sealed class PhoneNumber
    {
        public string Value { get; }

        public PhoneNumber(string value)
        {
            Value = value;
        }

        public static PhoneNumber Create(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number cannot be empty.");

            if (!IsValid(phoneNumber))
                throw new ArgumentException("Phone number format is invalid.");

            return new PhoneNumber(phoneNumber.Trim());
        }

        private static bool IsValid(string phoneNumber)
        {
            return Regex.IsMatch(
                phoneNumber,
                @"^[\d\s\-\+\(\)]+$"
            );
        }

        public override string ToString() => Value;

        public override bool Equals(object? obj)
             => obj is PhoneNumber other && Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();
    }
}
