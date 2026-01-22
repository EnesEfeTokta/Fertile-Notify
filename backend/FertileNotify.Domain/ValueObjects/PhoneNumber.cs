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

            var cleanedNumber = phoneNumber.Trim();
            if (!IsValid(cleanedNumber))
                throw new ArgumentException("Phone number format is invalid.");

            return new PhoneNumber(cleanedNumber);
        }

        private static bool IsValid(string phoneNumber)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(
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
