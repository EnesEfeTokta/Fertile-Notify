namespace FertileNotify.Domain.ValueObjects
{
    public sealed class CompanyName
    {
        public string Name { get; }
        
        public CompanyName(string name)
        {
            Name = name;
        }

        public static CompanyName Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Company name cannot be empty.");

            if (!IsValid(name))
                throw new ArgumentException("Company name format is invalid.");

            return new CompanyName(name);
        }

        private static bool IsValid(string name) => name.Length >= 2;

        public override string ToString() => Name;

        public override bool Equals(object? obj)
            => obj is CompanyName other && Name == other.Name;

        public override int GetHashCode() => Name.GetHashCode();
    }
}
