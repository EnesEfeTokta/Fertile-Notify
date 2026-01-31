using FertileNotify.Domain.Exceptions;

namespace FertileNotify.Domain.ValueObjects
{
    public sealed class Password : IEquatable<Password>
    {
        public string Hash { get; }

        private Password(string hash)
        {
            Hash = hash;
        }

        public static Password Create(string plainTextPassword)
        {
            if(string.IsNullOrEmpty(plainTextPassword))
                throw new BusinessRuleException("Password cannot be empty.");

            if (string.IsNullOrWhiteSpace(plainTextPassword) || plainTextPassword.Length < 8)
                throw new BusinessRuleException("Password must be at least 8 characters long.");

            return new Password(BCrypt.Net.BCrypt.HashPassword(plainTextPassword));
        }

        public static Password FromHash(string hash) => new Password(hash);

        public bool Verify(string plainTextPassword)
            => BCrypt.Net.BCrypt.Verify(plainTextPassword, this.Hash);

        public override bool Equals(object? obj) => obj is Password other && Hash == other.Hash;
        public bool Equals(Password? other) => other != null && Hash == other.Hash;
        public override int GetHashCode() => Hash.GetHashCode();
    }
}
