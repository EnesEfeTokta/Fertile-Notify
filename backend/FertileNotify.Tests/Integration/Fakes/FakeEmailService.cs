using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Tests.Integration.Fakes
{
    public class FakeEmailService : IEmailService
    {
        public Task SendEmailAsync(EmailAddress to, string subject, string body)
            => Task.CompletedTask;
    }
}
