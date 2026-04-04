namespace FertileNotify.Application.Interfaces.Notifications
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailAddress to, string subject, string body);
    }
}