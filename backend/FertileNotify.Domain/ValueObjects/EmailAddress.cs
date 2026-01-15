namespace FertileNotify.Domain.ValueObjects
{
    public class EmailAddress
    {
        public string Value { get; private set; }

        public EmailAddress(string value)
        {
            // Add validation logic for email format if needed
            Value = value;
        }

        // Override ToString() for easy display
        public override string ToString()
        {
            return Value;
        }
    }
}