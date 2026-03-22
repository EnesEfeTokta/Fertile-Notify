using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.ValueObjects;
using System.Threading.Tasks;

namespace FertileNotify.Tests.Integration.Fakes;

public class FakeEmailService : IEmailService
{
    public Task SendEmailAsync(EmailAddress to, string subject, string body)
    {
        // Simulate email sending delay or simply do nothing
        return Task.CompletedTask;
    }
}
