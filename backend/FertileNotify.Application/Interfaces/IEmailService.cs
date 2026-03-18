using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailAddress to, string subject, string body);
    }
}