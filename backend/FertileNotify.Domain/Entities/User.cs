using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public EmailAddress Email { get; private set; }

        private User() { }

        public User(EmailAddress email)
        {
            Id = Guid.NewGuid();
            Email = email;
        }
    }
}
