namespace FertileNotify.Domain.ValueObjects
{
    public class EmailAddress
    {
        public string Value { get; }

        public EmailAddress(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email address cannot be empty.");

            if (!IsValidEmail(value))
                throw new ArgumentException("Invalid email address format.");

            Value = value;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}